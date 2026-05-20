using KSP.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace com.github.lhervier.ksp
{
    public class SteamInputGlobalSettings
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("GlobalSettings");

        private const string CONFIG_KEY_LOG_LEVEL = "SteamInput.LogLevel";
        private const string CONFIG_KEY_SHOW_LOGGING_ICON = "SteamInput.ShowLoggingIcon";
        private const string CONFIG_KEY_CONTROLLER_CONFIG_NAME = "SteamInput.ControllerConfigName";
        private const string CONFIG_KEY_PHYSICAL_ZONES = "SteamInput.PhysicalZones";
        private const char PHYSICAL_ZONES_SEPARATOR = ',';
        private static PluginConfiguration config;

        private static LogLevel _logLevel = LogLevel.Info;
        private static bool _showLoggingIcon;
        private static string _controllerConfigName = string.Empty;
        private static List<GamepadZone> _physicalZones = new List<GamepadZone>();

        public static EventVoid OnConfigurationChanged = new EventVoid("SteamInputGlobalSettings.ConfigurationChanged");

        // =======================================================================

        private static void LoadLogLevel()
        {
            _logLevel = (LogLevel) Enum.Parse(
                typeof(LogLevel),
                config.GetValue(CONFIG_KEY_LOG_LEVEL, LogLevel.Info.ToString())
            );
        }

        private static void SaveLogLevel()
        {
            config.SetValue(CONFIG_KEY_LOG_LEVEL, _logLevel.ToString());
        }

        public static LogLevel GetLogLevel()
        {
            return _logLevel;
        }

        public static void SetLogLevel(LogLevel level)
        {
            LOGGER.LogDebug($"Setting log level to {level}");
            _logLevel = level;
            Save();
        }

        // =======================================================================

        private static void LoadShowLoggingIcon()
        {
            _showLoggingIcon = config.GetValue(CONFIG_KEY_SHOW_LOGGING_ICON, false);
        }

        private static void SaveShowLoggingIcon()
        {
            config.SetValue(CONFIG_KEY_SHOW_LOGGING_ICON, _showLoggingIcon);
        }

        public static bool GetShowLoggingIcon()
        {
            return _showLoggingIcon;
        }

        public static void SetShowLoggingIcon(bool show)
        {
            LOGGER.LogDebug($"Setting show logging icon to {show}");
            _showLoggingIcon = show;
            Save();
        }

        // =======================================================================

        private static void LoadControllerConfigName()
        {
            _controllerConfigName = config.GetValue(CONFIG_KEY_CONTROLLER_CONFIG_NAME, string.Empty);
        }

        private static void SaveControllerConfigName()
        {
            config.SetValue(CONFIG_KEY_CONTROLLER_CONFIG_NAME, _controllerConfigName ?? string.Empty);
        }

        public static string GetControllerConfigName()
        {
            return _controllerConfigName ?? string.Empty;
        }

        public static void SetControllerConfigName(string configName)
        {
            LOGGER.LogDebug($"Setting controller config name to {configName}");
            _controllerConfigName = configName ?? string.Empty;
            Save();
        }

        // =======================================================================

        private static void LoadPhysicalZones()
        {
            string raw = config.GetValue(
                CONFIG_KEY_PHYSICAL_ZONES,
                string.Join(PHYSICAL_ZONES_SEPARATOR.ToString(), GamepadZone.All.Select(z => z.ToString()))
            );

            if (string.IsNullOrEmpty(raw))
            {
                _physicalZones = new List<GamepadZone>(GamepadZone.All);
                return;
            }

            _physicalZones = new List<GamepadZone>();
            foreach (string part in raw.Split(PHYSICAL_ZONES_SEPARATOR))
            {
                string zone = part.Trim();
                if (string.IsNullOrEmpty(zone))
                {
                    continue;
                }
                if( GamepadZone.TryParse(zone, out GamepadZone gpZone) ) {
                    _physicalZones.Add(gpZone);
                }
            }
            if( _physicalZones.Count == 0 ) {
                _physicalZones = new List<GamepadZone>(GamepadZone.All);
            }
        }

        private static void SavePhysicalZones()
        {
            config.SetValue(
                CONFIG_KEY_PHYSICAL_ZONES, 
                string.Join(PHYSICAL_ZONES_SEPARATOR.ToString(), _physicalZones)
            );
        }

        public static List<GamepadZone> GetPhysicalZones()
        {
            return new List<GamepadZone>(_physicalZones);
        }

        public static void SetPhysicalZones(List<GamepadZone> physicalZones)
        {
            LOGGER.LogDebug($"Setting physical zones to {string.Join(PHYSICAL_ZONES_SEPARATOR.ToString(), physicalZones)}");
            _physicalZones = new List<GamepadZone>(physicalZones);
            Save();
        }

        // =======================================================================

        public static void Load()
        {
            LOGGER.LogDebug("Loading global settings");

            config = PluginConfiguration.CreateForType<SteamInputGlobalSettings>();
            config.load();

            LoadLogLevel();
            LoadShowLoggingIcon();
            LoadControllerConfigName();
            LoadPhysicalZones();
            
            OnConfigurationChanged.Fire();
            LOGGER.LogDebug($"Loaded configuration");
        }

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
            SavePhysicalZones();

            config.save();
            OnConfigurationChanged.Fire();
            LOGGER.LogDebug($"Saved configuration");
        }
    }
}
