using System.Collections.Generic;
using System.IO;
using com.github.lhervier.ksp.Vdf;

namespace com.github.lhervier.ksp
{
    /// <summary>
    /// Loads and caches the controller VDF configured in game settings (absolute path).
    /// </summary>
    public static class SteamInputControllerVdf
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("ControllerVdf");

        private static Dictionary<string, object> _root;
        private static Dictionary<string, string> _actionSetTitles;
        private static string _loadedPath;
        private static string _lastError;

        /// <summary>Last load or parse error, or null if the root is available.</summary>
        public static string LastError => _lastError;

        /// <summary>Parsed VDF root, or null if unavailable.</summary>
        public static Dictionary<string, object> Root
        {
            get
            {
                EnsureLoaded();
                return _root;
            }
        }

        public static bool TryGetRoot(out Dictionary<string, object> root)
        {
            EnsureLoaded();
            root = _root;
            return root != null;
        }

        /// <summary>
        /// Localized title of an action set (e.g. MenuControls → "Menu"), from actions or action_layers.
        /// </summary>
        public static bool TryGetActionSetTitle(string actionSetName, out string title)
        {
            EnsureLoaded();
            title = null;
            if (string.IsNullOrEmpty(actionSetName) || _actionSetTitles == null)
            {
                return false;
            }
            return _actionSetTitles.TryGetValue(actionSetName, out title);
        }

        /// <summary>
        /// Title of the action set, or <paramref name="actionSetName"/> if unknown or VDF unavailable.
        /// </summary>
        public static string GetActionSetTitle(string actionSetName)
        {
            string title;
            if (TryGetActionSetTitle(actionSetName, out title))
            {
                return title;
            }
            return actionSetName;
        }

        /// <summary>Discards the cache and reloads from the configured path.</summary>
        public static void Reload()
        {
            _loadedPath = null;
            _root = null;
            _actionSetTitles = null;
            _lastError = null;
            EnsureLoaded(force: true);
        }

        /// <summary>Discards the cache without reloading.</summary>
        public static void Clear()
        {
            _loadedPath = null;
            _root = null;
            _actionSetTitles = null;
            _lastError = null;
        }

        private static void EnsureLoaded(bool force = false)
        {
            var path = GetConfiguredPath();

            if (!force && path == _loadedPath)
            {
                return;
            }

            _loadedPath = path;
            _root = null;
            _actionSetTitles = null;
            _lastError = null;

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (!Path.IsPathRooted(path))
            {
                _lastError = "Controller VDF path must be absolute: " + path;
                LOGGER.LogError(_lastError);
                return;
            }

            if (!File.Exists(path))
            {
                _lastError = "Controller VDF file not found: " + path;
                LOGGER.LogError(_lastError);
                return;
            }

            try
            {
                _root = VdfParser.ParseFile(path);
                BuildIndexes();
                LOGGER.LogInfo("Loaded controller VDF: " + path);
            }
            catch (VdfParseException ex)
            {
                _lastError = ex.Message;
                LOGGER.LogError("Failed to parse controller VDF: " + ex.Message);
            }
            catch (System.Exception ex)
            {
                _lastError = ex.Message;
                LOGGER.LogError("Failed to load controller VDF: " + ex.Message);
            }
        }

        private static void BuildIndexes()
        {
            _actionSetTitles = new Dictionary<string, string>();
            if (_root == null)
            {
                return;
            }

            var mappings = GetBlock(_root, "controller_mappings");
            if (mappings == null)
            {
                return;
            }

            IndexActionSetTitles(GetBlock(mappings, "actions"));
            IndexActionSetTitles(GetBlock(mappings, "action_layers"));
        }

        private static void IndexActionSetTitles(Dictionary<string, object> actionSets)
        {
            if (actionSets == null)
            {
                return;
            }

            foreach (var entry in actionSets)
            {
                var block = entry.Value as Dictionary<string, object>;
                if (block == null)
                {
                    continue;
                }

                var title = GetString(block, "title");
                if (title != null)
                {
                    _actionSetTitles[entry.Key] = title;
                }
            }
        }

        private static Dictionary<string, object> GetBlock(Dictionary<string, object> parent, string key)
        {
            if (parent == null)
            {
                return null;
            }

            object value;
            if (!parent.TryGetValue(key, out value))
            {
                return null;
            }

            return value as Dictionary<string, object>;
        }

        private static string GetString(Dictionary<string, object> parent, string key)
        {
            if (parent == null)
            {
                return null;
            }

            object value;
            if (!parent.TryGetValue(key, out value))
            {
                return null;
            }

            return value as string;
        }

        private static string GetConfiguredPath()
        {
            return SteamInputGlobalSettings.GetControllerVdfPath();
        }
    }
}
