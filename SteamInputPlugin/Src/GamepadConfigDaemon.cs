using System;
using System.Collections.Generic;
using System.IO;
using com.github.lhervier.ksp.model;
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

        /// <summary>Parsed VDF root, or null if unavailable.</summary>
        private Dictionary<string, object> _root = new Dictionary<string, object>();
        
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
                _root = new Dictionary<string, object>();
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
            var mappings = GetObject(_root, "controller_mappings");
            Dictionary<string, object> actions = GetObject(mappings, "actions");
            Dictionary<string, object> action = GetObject(actions, actionGroup.ToString());
            return new VdfAction
            {
                Label = GetString(action, "label"),
                LegacySet = GetString(action, "legacy_set")
            };
        }

        public VdfControllerMappings GetControllerMappings()
        {
            var mappings = GetObject(_root, "controller_mappings");
            return new VdfControllerMappings
            {
                Title = GetString(mappings, "title"),
                ControllerType = GetString(mappings, "controller_type"),
                Description = GetString(mappings, "description")
            };
        }
        
        /// <summary>
        /// Get all the physical zones defined for the given action group.
        /// </summary>
        /// <param name="actionGroup">The action group to get the physical zones for.</param>
        /// <returns>A list of all the physical zones defined for the given action group.</returns>
        public Dictionary<EGamepadZone, VdfPresetZone> GetPresetZones(EActionGroup actionGroup)
        {
            var mappings = GetObject(_root, "controller_mappings");
            List<object> presets = GetList(mappings, "preset");
            Dictionary<string, object> preset = null;
            foreach( object presetObject in presets ) {
                if( presetObject is Dictionary<string, object> presetData ) {
                    if( presetData.TryGetValue("name", out object name) && name is string nameString && nameString == actionGroup.ToString() ) {
                        preset = presetData;
                        break;
                    }
                }
            }
            if( preset == null ) {
                return new Dictionary<EGamepadZone, VdfPresetZone>();
            }
            
            Dictionary<string, object> groupSourceBindings = GetObject(preset, "group_source_bindings");
            Dictionary<EGamepadZone, VdfPresetZone> presetZones = new Dictionary<EGamepadZone, VdfPresetZone>();
            
            foreach( KeyValuePair<string, object> pair in groupSourceBindings ) {
                string groupId = pair.Key;
                if( !(pair.Value is string valueString) ) {
                    continue;
                }
                if( !ParseGroupBinding(valueString, out EGamepadZone zone, out bool modeShift) ) {
                    continue;
                }
                AddPresetZone(presetZones, zone, groupId, modeShift);
                if( zone == EGamepadZone.Switch ) {
                    AddPresetZone(presetZones, EGamepadZone.Bumpers, groupId, modeShift);
                }
            }

            return presetZones;
        }

        /// <summary>
        /// Get all the gamepad zones defined in the VDF.
        /// </summary>
        /// <returns>A list of all the gamepad zones defined in the VDF.</returns>
        public List<EGamepadZone> GetGamepadZones()
        {
            var mappings = GetObject(_root, "controller_mappings");
            List<object> presets = GetList(mappings, "preset");
            List<EGamepadZone> gamepadZones = new List<EGamepadZone>();
            foreach( object presetObject in presets ) {
                if( presetObject is Dictionary<string, object> presetData ) {
                    var bindings = GetObject(presetData, "group_source_bindings");
                    foreach( KeyValuePair<string, object> pair in bindings ) {
                        if( !(pair.Value is string valueString) ) {
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
            }
            return gamepadZones;
        }

        public VdfGroup GetGroup(string groupId)
        {
            var mappings = GetObject(_root, "controller_mappings");
            var groups = GetList(mappings, "group");
            Dictionary<string, object> vdfGroup = null;
            foreach( object groupObject in groups )
            {
                if( !(groupObject is Dictionary<string, object> g) ) continue;
                string gid = GetString(g, "id");
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
                Mode = GetString(vdfGroup, "mode"),
                Inputs = GetInputs(vdfGroup)
            };
            return group;
        }

        private List<VdfInput> GetInputs(Dictionary<string, object> vdfGroup)
        {
            List<VdfInput> inputs = new List<VdfInput>();

            Dictionary<string, object> vdfInputs = GetObject(vdfGroup, "inputs");
            foreach( string inputName in vdfInputs.Keys )
            {
                Dictionary<string, object> vdfInput = GetObject(vdfInputs, inputName);
                inputs.Add(
                    new VdfInput
                    {
                        Name = inputName,
                        Activators = GetActivators(vdfInput)
                    }
                );
            }
            return inputs;
        }

        private List<VdfActivator> GetActivators(Dictionary<string, object> vdfInput)
        {
            List<VdfActivator> activators = new List<VdfActivator>();
            Dictionary<string, object> vdfActivators = GetObject(vdfInput, "activators");
            foreach(string activatorName in vdfActivators.Keys )
            {
                List<object> vdfActivatorInstances = GetList(vdfActivators, activatorName);
                foreach( object vdfActivator in vdfActivatorInstances )
                {
                    if( vdfActivator is Dictionary<string, object> vdfActivatorDict )
                    {
                        activators.Add(
                            new VdfActivator
                            {
                                Name = activatorName,
                                Binding = GetBinding(vdfActivatorDict)
                            }
                        );
                    }
                }
            }

            return activators;
        }

        private VdfBinding GetBinding(Dictionary<string, object> vdfActivator)
        {
            Dictionary<string, object> vdfBindings = GetObject(vdfActivator, "bindings");
            string bindingString = GetString(vdfBindings, "binding");
            if( string.IsNullOrEmpty(bindingString) ) return null;

            VdfBinding binding = new VdfBinding();
            string[] parts = bindingString.Split(' ');
            if( parts.Length == 0 )
            {
                return binding;
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
            return binding;
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

        // ============================================================================
        // Helpers : VDF parsing
        // ============================================================================

        /// <summary>
        /// Get an object from the VDF.
        /// </summary>
        /// <param name="parent">The parent object to get the object from.</param>
        /// <param name="key">The key to get the object from.</param>
        /// <returns>The object from the VDF.</returns>
        private static Dictionary<string, object> GetObject(Dictionary<string, object> parent, string key)
        {
            if (parent == null)
            {
                throw new System.ArgumentNullException("parent");
            }

            if (!parent.TryGetValue(key, out object value))
            {
                return new Dictionary<string, object>();
            }

            if (value is Dictionary<string, object> block)
            {
                return block;
            }

            throw new System.InvalidOperationException("Expected Dictionnary, got " + value.GetType().Name);
        }

        /// <summary>
        /// Get a string from the VDF.
        /// </summary>
        /// <param name="parent">The parent object to get the string from.</param>
        /// <param name="key">The key to get the string from.</param>
        /// <returns>The string from the VDF.</returns>
        private static string GetString(Dictionary<string, object> parent, string key)
        {
            if (parent == null)
            {
                throw new System.ArgumentNullException("parent");
            }

            if (!parent.TryGetValue(key, out object value))
            {
                return "";
            }
            if (value is string str)
            {
                return str;
            }

            throw new System.InvalidOperationException("Expected String, got " + value.GetType().Name);
        }

        /// <summary>
        /// Get a list from the VDF.
        /// </summary>
        /// <param name="parent">The parent object to get the list from.</param>
        /// <param name="key">The key to get the list from.</param>
        /// <returns>The list from the VDF.</returns>
        private static List<object> GetList(Dictionary<string, object> parent, string key)
        {
            if (parent == null)
            {
                throw new System.ArgumentNullException("parent");
            }
            if (!parent.TryGetValue(key, out object value))
            {
                return new List<object>();
            }
            if (value is List<object> list)
            {
                return list;
            }

            return new List<object> { value };
        }
    }
}
