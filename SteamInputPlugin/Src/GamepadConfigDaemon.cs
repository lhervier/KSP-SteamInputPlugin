using System;
using System.Collections.Generic;
using System.IO;
using com.github.lhervier.ksp.model;
using com.github.lhervier.ksp.ui.model;
using com.github.lhervier.ksp.Vdf;
using UnityEngine;

namespace com.github.lhervier.ksp
{
    /// <summary>
    /// Loads and caches the controller VDF for the config name set in game settings.
    /// </summary>
    public class GamepadConfigDaemon : MonoBehaviour
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("GamepadConfigDaemon");

        private static GamepadConfigDaemon _instance;
        public static GamepadConfigDaemon Instance {
            get {
                return _instance;
            }
        }
        
        private string _lastPath = "";
        
        private DateTime _lastWriteTime = DateTime.MinValue;

        /// <summary>Last load or parse error, or null if the root is available.</summary>
        private string _lastError = null;
        public string LastError => _lastError;

        /// <summary>Parsed VDF root, or an empty object if unavailable.</summary>
        private VdfObject _root = new VdfObject();
        
        public EventVoid OnConfigLoaded = new EventVoid("GamepadConfigDaemon.OnConfigLoaded");
        public EventData<string> OnConfigLoadError = new EventData<string>("GamepadConfigDaemon.OnConfigLoadError");

        // =======================================================================
        //              Unity Lifecycle
        // =======================================================================

        public void Awake() 
        {
            DontDestroyOnLoad(this);
            _instance = this;
            LOGGER.LogInfo("Awaked");
        }

        public void Start() 
        {
            LOGGER.LogInfo("Start");
            SteamInputGlobalSettings.OnGlobalSettingsChanged.Add(this.UpdateConfiguration);
            this.UpdateConfiguration(UpdatedConfiguration.ALL);
            LOGGER.LogInfo("Started");
        }

        public void OnDestroy() 
        {
            LOGGER.LogInfo("OnDestroy");
            SteamInputGlobalSettings.OnGlobalSettingsChanged.Remove(this.UpdateConfiguration);
            _instance = null;
            LOGGER.LogInfo("Destroyed");
        }

        // =======================================================================
        //              Reload and Clear
        // =======================================================================

        /// <summary>
        /// Reload the gamepad configuration from the configured path
        /// if the path has changed or the file has been modified.
        /// </summary>
        /// <param name="updateFlags">The update flags.</param>
        public void UpdateConfiguration(int updateFlags)
        {
            if( (updateFlags & UpdatedConfiguration.CONTROLLER_CONFIG_NAME) == 0 ) {
                return;
            }
            var configName = SteamInputGlobalSettings.GetControllerConfigName();

            // Empty path is not an error, just an empty config
            if (string.IsNullOrEmpty(configName))
            {
                bool hadConfig = _lastPath != "";
                _root = new VdfObject();
                _lastError = null;
                _lastPath = "";
                _lastWriteTime = DateTime.MinValue;
                if (hadConfig)
                {
                    OnConfigLoaded.Fire();
                }
                return;
            }

            // Resolve the path to the VDF file
            if (!GamepadConfigPathResolver.TryResolve(configName, out string path, out string resolveError))
            {
                _lastPath = "";
                _lastWriteTime = DateTime.MinValue;
                _lastError = resolveError;
                LOGGER.LogError(_lastError);
                OnConfigLoadError.Fire(_lastError);
                return;
            }

            // Don't reload if the file has not changed
            if (path == _lastPath && File.GetLastWriteTime(path) == _lastWriteTime)
            {
                if (_lastError != null)
                {
                    _lastError = null;
                    OnConfigLoaded.Fire();
                }
                return;
            }

            // Reload the file
            try
            {
                _root = VdfParser.ParseFile(path);
            }
            catch (VdfParseException ex)
            {
                _lastError = "Failed to parse gamepad VDF: " + ex.Message;
                OnConfigLoadError.Fire(_lastError);
                LOGGER.LogError(_lastError);
                return;
            }
            catch (System.Exception ex)
            {
                _lastError = "Failed to load gamepad VDF: " + ex.Message;
                OnConfigLoadError.Fire(_lastError);
                LOGGER.LogError(_lastError);
                return;
            }
            
            _lastPath = path;
            _lastWriteTime = File.GetLastWriteTime(path);
            _lastError = null;
            OnConfigLoaded.Fire();
            LOGGER.LogInfo("Loaded gamepad VDF: " + path);
        }

        // ============================================================================

        public VdfAction GetAction(EActionGroup actionGroup)
        {
            VdfObject mappings = _root.GetObject("controller_mappings");
            VdfObject actions = mappings.GetObject("actions");
            VdfObject action = actions.GetObject(actionGroup.ToString());
            return new VdfAction
            {
                Title = action.GetString("title"),
                LegacySet = action.GetString("legacy_set")
            };
        }

        public VdfControllerMappings GetControllerMappings()
        {
            VdfObject mappings = _root.GetObject("controller_mappings");
            if( !EControllerType.TryParse(mappings.GetString("controller_type"), out EControllerType controllerType) )
            {
                controllerType = null;
            }
            return new VdfControllerMappings
            {
                Title = mappings.GetString("title"),
                ControllerType = controllerType,
                Description = mappings.GetString("description")
            };
        }
        
        /// <summary>
        /// Get all the physical zones defined for the given action group.
        /// </summary>
        /// <param name="actionGroup">The action group to get the physical zones for.</param>
        /// <returns>A list of all the physical zones defined for the given action group.</returns>
        public Dictionary<EGamepadZone, VdfPresetZone> GetPresetZones(EActionGroup actionGroup)
        {
            VdfObject mappings = _root.GetObject("controller_mappings");
            VdfArray presets = mappings.GetArray("preset");
            VdfObject preset = null;
            foreach( VdfObject presetData in presets.Objects() ) {
                if( presetData.TryGetString("name", out string nameString) && nameString == actionGroup.ToString() ) {
                    preset = presetData;
                    break;
                }
            }
            if( preset == null ) {
                return new Dictionary<EGamepadZone, VdfPresetZone>();
            }

            VdfObject groupSourceBindings = preset.GetObject("group_source_bindings");
            Dictionary<EGamepadZone, VdfPresetZone> presetZones = new Dictionary<EGamepadZone, VdfPresetZone>();

            foreach( string groupId in groupSourceBindings.Keys ) {
                if( !groupSourceBindings.TryGetString(groupId, out string valueString) ) {
                    continue;
                }
                if( !ParseGroupBinding(valueString, out EGamepadZone zone, out bool modeShift) ) {
                    continue;
                }
                AddPresetZone(presetZones, zone, groupId, modeShift);
            }

            return presetZones;
        }

        /// <summary>
        /// Get all the gamepad zones defined in the VDF.
        /// </summary>
        /// <returns>A list of all the gamepad zones defined in the VDF.</returns>
        public List<EGamepadZone> GetGamepadZones()
        {
            VdfObject mappings = _root.GetObject("controller_mappings");
            VdfArray presets = mappings.GetArray("preset");
            List<EGamepadZone> gamepadZones = new List<EGamepadZone>();
            foreach( VdfObject presetData in presets.Objects() ) {
                VdfObject bindings = presetData.GetObject("group_source_bindings");
                foreach( string groupId in bindings.Keys ) {
                    if( !bindings.TryGetString(groupId, out string valueString) ) {
                        continue;
                    }
                    if( !ParseGroupBinding(valueString, out EGamepadZone zone, out bool _) ) {
                        continue;
                    }
                    if( gamepadZones.Contains(zone) ) {
                        continue;
                    }
                    gamepadZones.Add(zone);
                }
            }
            return gamepadZones;
        }

        public VdfGroup GetGroup(string groupId)
        {
            VdfObject mappings = _root.GetObject("controller_mappings");
            VdfArray groups = mappings.GetArray("group");
            VdfObject vdfGroup = null;
            foreach( VdfObject g in groups.Objects() )
            {
                string gid = g.GetString("id");
                if( string.IsNullOrEmpty(gid) ) continue;
                if( gid == groupId )
                {
                    vdfGroup = g;
                    break;
                }
            }
            if( vdfGroup == null )
            {
                return null;
            }

            VdfGroup group = new VdfGroup
            {
                GroupId = groupId,
                Mode = vdfGroup.GetString("mode"),
                Inputs = GetInputs(vdfGroup)
            };
            return group;
        }

        private List<VdfInput> GetInputs(VdfObject vdfGroup)
        {
            List<VdfInput> inputs = new List<VdfInput>();

            VdfObject vdfInputs = vdfGroup.GetObject("inputs");
            foreach( string inputName in vdfInputs.Keys )
            {
                if( !EInput.TryParse(inputName, out EInput input) )
                {
                    input = null;
                }
                // The same input may be declared several times (one block per activator type);
                // GetArray returns all the declarations so their activators can be merged.
                inputs.Add(
                    new VdfInput
                    {
                        Name = input,
                        Activators = GetActivators(vdfInputs.GetArray(inputName))
                    }
                );
            }
            return inputs;
        }

        private List<VdfActivator> GetActivators(VdfArray vdfInputs)
        {
            List<VdfActivator> activators = new List<VdfActivator>();
            Dictionary<EActivator, VdfActivator> byName = new Dictionary<EActivator, VdfActivator>();

            foreach( VdfObject vdfInput in vdfInputs.Objects() )
            {
                VdfObject vdfActivators = vdfInput.GetObject("activators");
                foreach( string activatorName in vdfActivators.Keys )
                {
                    if( !EActivator.TryParse(activatorName, out EActivator act) )
                    {
                        continue;
                    }

                    // Merge bindings of the same activator across every declaration of the input.
                    if( !byName.TryGetValue(act, out VdfActivator activator) )
                    {
                        activator = new VdfActivator { Name = act, Bindings = new List<VdfBinding>() };
                        byName[act] = activator;
                        activators.Add(activator);
                    }

                    foreach( VdfObject vdfActivator in vdfActivators.GetArray(activatorName).Objects() )
                    {
                        activator.Bindings.AddRange(GetBindings(vdfActivator));
                    }
                }
            }

            return activators;
        }

        private List<VdfBinding> GetBindings(VdfObject vdfActivator)
        {
            List<VdfBinding> bindings = new List<VdfBinding>();
            VdfObject vdfBindings = vdfActivator.GetObject("bindings");
            VdfArray bindingValues = vdfBindings.GetArray("binding");
            foreach( string bindingString in bindingValues.Strings() )
            {
                if( string.IsNullOrEmpty(bindingString) ) continue;

                VdfBinding binding = new VdfBinding();
                string[] parts = bindingString.Split(' ');
                if( parts.Length == 0 )
                {
                    bindings.Add(binding);
                    continue;
                }

                binding.ModeShift = parts[0] == "mode_shift";

                if( binding.ModeShift )
                {
                    if( parts.Length > 1 )
                    {
                        EGamepadZone.TryParse(parts[1], out EGamepadZone zone);
                        binding.Zone = zone;
                    }

                    if( parts.Length > 2 )
                    {
                        binding.GroupId = parts[2];
                    }
                }
                else
                {
                    binding.EventType = parts[0];
                    if( parts.Length > 1 )
                    {
                        string right = bindingString.Substring(binding.EventType.Length + 1);
                        string[] rightParts = right.Split(',');

                        if( rightParts.Length > 0 )
                        {
                            binding.Action = rightParts[0];
                        }
                        if( rightParts.Length > 1 )
                        {
                            binding.Label = rightParts[1].Trim();
                        }
                    }
                }
                bindings.Add(binding);
            }
            return bindings;
        }

        // ============================================================================
        // Helpers
        // ============================================================================

        /// <summary>
        /// Parse a group binding string into a gamepad zone and mode shift flag.
        /// </summary>
        /// <param name="groupBinding">The group binding string to parse.</param>
        /// <param name="zone">The parsed gamepad zone.</param>
        /// <param name="modeShift">The mode shift flag.</param>
        /// <returns>True if the group binding string was parsed successfully, false otherwise.</returns>
        private bool ParseGroupBinding(string groupBinding, out EGamepadZone zone, out bool modeShift) {
            List<string> parts = new List<string>(groupBinding.Split(' '));
            zone = null;
            modeShift = false;

            if( !parts.Contains("active") ) {
                return false;
            }
            parts.Remove("active");

            modeShift = parts.Contains("modeshift");
            parts.Remove("modeshift");

            if( parts.Count == 0 ) {
                return false;
            }
            string name = parts[0];

            if( EGamepadZone.TryParse(name, out EGamepadZone z) ) {
                zone = z;
            } else {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Add a physical zone to the dictionary.
        /// </summary>
        /// <param name="presetZones">The dictionary to add the physical zone to.</param>
        /// <param name="zone">The gamepad zone to add.</param>
        /// <param name="groupId">The group id to add.</param>
        /// <param name="modeShift">The mode shift flag to add.</param>
        private void AddPresetZone(
            Dictionary<EGamepadZone, VdfPresetZone> presetZones, 
            EGamepadZone zone, 
            string groupId, 
            bool modeShift
        ) {
            if( !presetZones.ContainsKey(zone) ) {
                presetZones[zone] = new VdfPresetZone { 
                    Zone = zone, 
                };
            }
            if( modeShift ) {
                presetZones[zone].ModeshiftGroupIds.Add(groupId);
            } else {
                presetZones[zone].GroupId = groupId;
            }
        }

    }
}
