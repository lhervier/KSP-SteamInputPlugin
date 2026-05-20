using KSP.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace com.github.lhervier.ksp
{
    public static class UpdatedConfiguration
    {
        public static readonly int LOG_LEVEL = 1;
        public static readonly int SHOW_LOGGING_ICON = 2;
        public static readonly int CONTROLLER_CONFIG_NAME = 4;
        public static readonly int ORDERED_GAMEPAD_ZONES = 8;
        public static readonly int VISIBLE_GAMEPAD_ZONES = 16;

        public static readonly int ALL = LOG_LEVEL | SHOW_LOGGING_ICON | CONTROLLER_CONFIG_NAME | ORDERED_GAMEPAD_ZONES | VISIBLE_GAMEPAD_ZONES;
    }

    public class SteamInputGlobalSettings
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("GlobalSettings");

        private const string CONFIG_KEY_LOG_LEVEL = "SteamInput.LogLevel";
        private const string CONFIG_KEY_SHOW_LOGGING_ICON = "SteamInput.ShowLoggingIcon";
        private const string CONFIG_KEY_CONTROLLER_CONFIG_NAME = "SteamInput.ControllerConfigName";
        private const string CONFIG_KEY_ORDERED_GAMEPAD_ZONES = "SteamInput.OrderedGamepadZones";
        private const string CONFIG_KEY_VISIBLE_GAMEPAD_ZONES = "SteamInput.VisibleGamepadZones";
        private const char GAMEPAD_ZONES_SEPARATOR = ',';
        private static PluginConfiguration config;

        private static LogLevel _logLevel = LogLevel.Info;
        private static bool _showLoggingIcon = false;
        private static string _controllerConfigName = string.Empty;
        private static List<GamepadZone> _orderedGamepadZones = new List<GamepadZone>();
        private static List<GamepadZone> _visibleGamepadZones = new List<GamepadZone>();

        public static EventData<int> OnGlobalSettingsChanged = new EventData<int>("SteamInputGlobalSettings.ConfigurationChanged");

        // =======================================================================

        /// <summary>
        /// Load the log level from the configuration.
        /// </summary>
        /// <returns>The update flags.</returns>
        private static int LoadLogLevel()
        {
            return _SetLogLevel(
                (LogLevel) Enum.Parse(
                    typeof(LogLevel),
                    config.GetValue(CONFIG_KEY_LOG_LEVEL, LogLevel.Info.ToString())
                )
            );
        }

        /// <summary>
        /// Save the log level to the configuration.
        /// </summary>
        private static void SaveLogLevel()
        {
            config.SetValue(CONFIG_KEY_LOG_LEVEL, _logLevel.ToString());
        }

        /// <summary>
        /// Get the log level.
        /// </summary>
        /// <returns>The log level.</returns>
        public static LogLevel GetLogLevel()
        {
            return _logLevel;
        }

        /// <summary>
        /// Set the log level.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <returns>The update flags.</returns>
        public static int _SetLogLevel(LogLevel level)
        {
            LOGGER.LogDebug($"Setting log level to {level}");
            if( _logLevel == level ) {
                return 0;
            }
            _logLevel = level;
            return UpdatedConfiguration.LOG_LEVEL;
        }

        /// <summary>
        /// Set the log level.
        /// </summary>
        /// <param name="level">The log level.</param>
        public static void SetLogLevel(LogLevel level)
        {
            OnGlobalSettingsChanged.Fire(
                _SetLogLevel(level)
            );
        }

        // =======================================================================

        /// <summary>
        /// Load the show logging icon from the configuration.
        /// </summary>
        /// <returns>The update flags.</returns>
        private static int LoadShowLoggingIcon()
        {
            return _SetShowLoggingIcon(
                config.GetValue(CONFIG_KEY_SHOW_LOGGING_ICON, false)
            );
        }

        /// <summary>
        /// Save the show logging icon to the configuration.
        /// </summary>
        private static void SaveShowLoggingIcon()
        {
            config.SetValue(CONFIG_KEY_SHOW_LOGGING_ICON, _showLoggingIcon);
        }

        /// <summary>
        /// Get the show logging icon.
        /// </summary>
        /// <returns>The show logging icon.</returns>
        public static bool GetShowLoggingIcon()
        {
            return _showLoggingIcon;
        }

        /// <summary>
        /// Set the show logging icon.
        /// </summary>
        /// <param name="show">The show logging icon.</param>
        public static int _SetShowLoggingIcon(bool show)
        {
            if( _showLoggingIcon == show ) {
                return 0;
            }
            LOGGER.LogDebug($"Setting show logging icon to {show}");
            _showLoggingIcon = show;
            return UpdatedConfiguration.SHOW_LOGGING_ICON;
        }

        /// <summary>
        /// Set the show logging icon.
        /// </summary>
        /// <param name="show">The show logging icon.</param>
        public static void SetShowLoggingIcon(bool show)
        {
            OnGlobalSettingsChanged.Fire(
                _SetShowLoggingIcon(show)
            );
        }

        // =======================================================================

        /// <summary>
        /// Load the controller config name from the configuration.
        /// </summary>
        private static int LoadControllerConfigName()
        {
            return _SetControllerConfigName(
                config.GetValue(CONFIG_KEY_CONTROLLER_CONFIG_NAME, string.Empty)
            );
        }

        /// <summary>
        /// Save the controller config name to the configuration.
        /// </summary>
        private static void SaveControllerConfigName()
        {
            config.SetValue(CONFIG_KEY_CONTROLLER_CONFIG_NAME, _controllerConfigName ?? string.Empty);
        }

        /// <summary>
        /// Get the controller config name.
        /// </summary>
        /// <returns>The controller config name.</returns>
        public static string GetControllerConfigName()
        {
            return _controllerConfigName ?? string.Empty;
        }

        /// <summary>
        /// Set the controller config name.
        /// </summary>
        /// <param name="configName">The controller config name.</param>
        /// <returns>The update flags.</returns>
        public static int _SetControllerConfigName(string configName)
        {
            if( _controllerConfigName == configName ) {
                return 0;
            }
            LOGGER.LogDebug($"Setting controller config name to {configName}");
            _controllerConfigName = configName ?? string.Empty;
            return UpdatedConfiguration.CONTROLLER_CONFIG_NAME;
        }

        /// <summary>
        /// Set the controller config name.
        /// </summary>
        /// <param name="configName">The controller config name.</param>
        public static void SetControllerConfigName(string configName)
        {
            OnGlobalSettingsChanged.Fire(
                _SetControllerConfigName(configName)
            );
        }

        // =======================================================================

        /// <summary>
        /// Load the ordered gamepad zones from the configuration.
        /// </summary>
        private static int LoadOrderedGamepadZones()
        {
            string raw = config.GetValue(
                CONFIG_KEY_ORDERED_GAMEPAD_ZONES,
                string.Join(GAMEPAD_ZONES_SEPARATOR.ToString(), GamepadZone.All.Select(z => z.ToString()))
            );
            return _SetOrderedGamepadZones(
                ParseGamepadZones(raw)
            );
        }

        /// <summary>
        /// Save the ordered gamepad zones to the configuration.
        /// </summary>
        private static void SaveOrderedGamepadZones()
        {
            config.SetValue(
                CONFIG_KEY_ORDERED_GAMEPAD_ZONES, 
                string.Join(GAMEPAD_ZONES_SEPARATOR.ToString(), _orderedGamepadZones)
            );
        }

        /// <summary>
        /// Get the ordered gamepad zones.
        /// </summary>
        /// <returns>The ordered gamepad zones.</returns>
        public static List<GamepadZone> GetOrderedGamepadZones()
        {
            return new List<GamepadZone>(_orderedGamepadZones);
        }

        /// <summary>
        /// Set the ordered gamepad zones.
        /// </summary>
        /// <param name="orderedGamepadZones">The ordered gamepad zones.</param>
        /// <returns>The update flags.</returns>
        public static int _SetOrderedGamepadZones(List<GamepadZone> orderedGamepadZones)
        {
            LOGGER.LogDebug($"Setting ordered gamepad zones to {string.Join(GAMEPAD_ZONES_SEPARATOR.ToString(), orderedGamepadZones)}");
            
            bool sameZoneSet = _orderedGamepadZones.Count == orderedGamepadZones.Count
                && !_orderedGamepadZones.Except(orderedGamepadZones).Any()
                && !orderedGamepadZones.Except(_orderedGamepadZones).Any();

            // If the list of gamepad zones has changed (indifferent to the order), set all zones as visible
            if( !sameZoneSet ) {
                _orderedGamepadZones = new List<GamepadZone>(orderedGamepadZones);
                _visibleGamepadZones = new List<GamepadZone>(orderedGamepadZones);
                return UpdatedConfiguration.VISIBLE_GAMEPAD_ZONES | UpdatedConfiguration.ORDERED_GAMEPAD_ZONES;
            }
            
            // If the order of the elements has changed, just update the list
            if( !_orderedGamepadZones.SequenceEqual(orderedGamepadZones) ) {
                _orderedGamepadZones = new List<GamepadZone>(orderedGamepadZones);
                return UpdatedConfiguration.ORDERED_GAMEPAD_ZONES;
            }
            return 0;
        }

        /// <summary>
        /// Set the ordered gamepad zones.
        /// </summary>
        /// <param name="orderedGamepadZones">The ordered gamepad zones.</param>
        public static void SetOrderedGamepadZones(List<GamepadZone> orderedGamepadZones)
        {
            OnGlobalSettingsChanged.Fire(
                _SetOrderedGamepadZones(orderedGamepadZones)
            );
        }

        // =======================================================================

        /// <summary>
        /// Load the visible gamepad zones from the configuration.
        /// </summary>
        private static int LoadVisibleGamepadZones()
        {
            string raw = config.GetValue(
                CONFIG_KEY_VISIBLE_GAMEPAD_ZONES,
                string.Join(GAMEPAD_ZONES_SEPARATOR.ToString(), GamepadZone.All.Select(z => z.ToString()))
            );
            return _SetVisibleGamepadZones(
                ParseGamepadZones(raw)
            );
        }

        /// <summary>
        /// Save the visible gamepad zones to the configuration.
        /// </summary>
        private static void SaveVisibleGamepadZones()
        {
            config.SetValue(
                CONFIG_KEY_VISIBLE_GAMEPAD_ZONES, 
                string.Join(GAMEPAD_ZONES_SEPARATOR.ToString(), _visibleGamepadZones)
            );
        }

        /// <summary>
        /// Get the visible gamepad zones.
        /// </summary>
        /// <returns>The visible gamepad zones.</returns>
        public static List<GamepadZone> GetVisibleGamepadZones()
        {
            return new List<GamepadZone>(_visibleGamepadZones);
        }

        /// <summary>
        /// Set the visible gamepad zones.
        /// </summary>
        /// <param name="visibleGamepadZones">The visible gamepad zones.</param>
        /// <returns>The update flags.</returns>
        public static int _SetVisibleGamepadZones(List<GamepadZone> visibleGamepadZones)
        {
            LOGGER.LogDebug($"Setting visible gamepad zones to {string.Join(GAMEPAD_ZONES_SEPARATOR.ToString(), visibleGamepadZones)}");
            
            if( _visibleGamepadZones.Count == visibleGamepadZones.Count && _visibleGamepadZones.SequenceEqual(visibleGamepadZones) ) {
                return 0;
            }
            _visibleGamepadZones = new List<GamepadZone>(visibleGamepadZones);
            return UpdatedConfiguration.VISIBLE_GAMEPAD_ZONES;
        }

        /// <summary>
        /// Set the visible gamepad zones.
        /// </summary>
        /// <param name="visibleGamepadZones">The visible gamepad zones.</param>
        public static void SetVisibleGamepadZones(List<GamepadZone> visibleGamepadZones)
        {
            OnGlobalSettingsChanged.Fire(
                _SetVisibleGamepadZones(visibleGamepadZones)
            );
        }

        // =======================================================================

        /// <summary>
        /// Load the global settings from the configuration.
        /// </summary>
        public static void Load()
        {
            LOGGER.LogDebug("Loading global settings");

            config = PluginConfiguration.CreateForType<SteamInputGlobalSettings>();
            config.load();

            int updateFlags = 0;
            updateFlags |= LoadLogLevel();
            updateFlags |= LoadShowLoggingIcon();
            updateFlags |= LoadControllerConfigName();
            updateFlags |= LoadOrderedGamepadZones();
            updateFlags |= LoadVisibleGamepadZones();
            
            OnGlobalSettingsChanged.Fire(updateFlags);
            LOGGER.LogDebug($"Loaded configuration");
        }

        /// <summary>
        /// Save the global settings to the configuration.
        /// </summary>
        public static void Save()
        {
            LOGGER.LogDebug("Saving global settings");
            if (config == null)
            {
                config = PluginConfiguration.CreateForType<SteamInputGlobalSettings>();
            }

            SaveLogLevel();
            SaveShowLoggingIcon();
            SaveControllerConfigName();
            SaveOrderedGamepadZones();
            SaveVisibleGamepadZones();

            config.save();
            LOGGER.LogDebug($"Saved configuration");
        }

        // =======================================================================
        // Helpers
        // =======================================================================

        /// <summary>
        /// Parse a list of gamepad zones from a string.
        /// </summary>
        /// <param name="raw">The string to parse.</param>
        /// <returns>The list of gamepad zones.</returns>
        private static List<GamepadZone> ParseGamepadZones(string raw)
        {
            if( string.IsNullOrEmpty(raw) ) {
                return new List<GamepadZone>();
            }
            List<GamepadZone> gamepadZones = new List<GamepadZone>();
            foreach (string part in raw.Split(GAMEPAD_ZONES_SEPARATOR))
            {
                string zone = part.Trim();
                if (string.IsNullOrEmpty(zone))
                {
                    continue;
                }
                if( GamepadZone.TryParse(zone, out GamepadZone gpZone) ) {
                    gamepadZones.Add(gpZone);
                }
            }
            return gamepadZones;
        }
    }
}
