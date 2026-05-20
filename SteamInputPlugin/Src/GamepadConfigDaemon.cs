using System;
using System.Collections.Generic;
using System.IO;
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
        public Dictionary<string, object> Root
        {
            get
            {
                UpdateConfiguration();
                return _root;
            }
        }

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
            SteamInputGlobalSettings.OnConfigurationChanged.Add(this.UpdateConfiguration);
            this.UpdateConfiguration();
            LOGGER.LogInfo("Started");
        }

        public void OnDestroy() 
        {
            LOGGER.LogInfo("OnDestroy");
            SteamInputGlobalSettings.OnConfigurationChanged.Remove(this.UpdateConfiguration);
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
        public void UpdateConfiguration()
        {
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

        public Dictionary<string, object> GetAction(ActionGroup actionGroup)
        {
            var mappings = GetObject(_root, "controller_mappings");
            Dictionary<string, object> actions = GetObject(mappings, "actions");
            return GetObject(actions, actionGroup.ToString());
        }

        public string GetControllerType() {
            var mappings = GetObject(_root, "controller_mappings");
            return GetString(mappings, "controller_type");
        }

        public List<PhysicalZone> GetPhysicalZones(ActionGroup actionGroup)
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
                return new List<PhysicalZone>();
            }
            
            Dictionary<string, object> groupSourceBindings = GetObject(preset, "group_source_bindings");
            Dictionary<string, PhysicalZone> physicalZones = new Dictionary<string, PhysicalZone>();
            
            foreach( KeyValuePair<string, object> pair in groupSourceBindings ) {
                if( !(pair.Value is string valueString) ) {
                    continue;
                }
                List<string> parts = new List<string>(valueString.Split(' '));

                if( !parts.Contains("active") ) {
                    continue;
                }
                parts.Remove("active");

                bool modeShift = parts.Contains("modeshift");
                parts.Remove("modeshift");

                if( parts.Count == 0 ) {
                    continue;
                }

                string name = parts[0];
                string groupId = pair.Key;
                AddPhysicalZone(physicalZones, name, groupId, modeShift);
                if( name == "switch") {
                    AddPhysicalZone(physicalZones, "bumpers", groupId, modeShift);
                }
            }

            return new List<PhysicalZone>(physicalZones.Values);
        }

        private void AddPhysicalZone(
            Dictionary<string, PhysicalZone> physicalZones, 
            string name, 
            string groupId, 
            bool modeShift
        ) {
            if( !physicalZones.ContainsKey(name) ) {
                physicalZones[name] = new PhysicalZone { 
                    Name = name, 
                };
            }
            if( modeShift ) {
                physicalZones[name].ModeshiftGroupId = groupId;
            } else {
                physicalZones[name].GroupId = groupId;
            }
        }

        // ============================================================================

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
