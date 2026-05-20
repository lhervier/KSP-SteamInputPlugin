using KSP.IO;
using System;

namespace com.github.lhervier.ksp
{
    public class SteamInputGlobalSettings
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("GlobalSettings");
        private const string CONFIG_KEY_LOG_LEVEL = "SteamInput.LogLevel";
        private const string CONFIG_KEY_SHOW_LOGGING_ICON = "SteamInput.ShowLoggingIcon";
        private const string CONFIG_KEY_CONTROLLER_VDF_PATH = "SteamInput.ControllerVdfPath";
        private static PluginConfiguration config;

        private static LogLevel _logLevel = LogLevel.Info;
        private static bool _showLoggingIcon;
        private static string _controllerVdfPath = string.Empty;

        public static EventVoid OnConfigurationChanged = new EventVoid("SteamInputGlobalSettings.ConfigurationChanged");

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

        public static string GetControllerVdfPath()
        {
            return _controllerVdfPath ?? string.Empty;
        }

        public static void SetControllerVdfPath(string path)
        {
            var normalized = (path ?? string.Empty).Trim();
            LOGGER.LogDebug($"Setting controller VDF path to {normalized}");
            _controllerVdfPath = normalized;
            Save();
        }

        public static void Load()
        {
            LOGGER.LogDebug("Loading global settings");

            config = PluginConfiguration.CreateForType<SteamInputGlobalSettings>();
            config.load();

            _logLevel = (LogLevel)Enum.Parse(
                typeof(LogLevel),
                config.GetValue(CONFIG_KEY_LOG_LEVEL, LogLevel.Info.ToString())
            );
            _showLoggingIcon = config.GetValue(CONFIG_KEY_SHOW_LOGGING_ICON, false);
            _controllerVdfPath = config.GetValue(CONFIG_KEY_CONTROLLER_VDF_PATH, string.Empty);

            OnConfigurationChanged.Fire();
            LOGGER.LogDebug($"Loaded log level: {_logLevel}, showLoggingIcon: {_showLoggingIcon}, controllerVdfPath: {_controllerVdfPath}");
        }

        public static void Save()
        {
            LOGGER.LogDebug("Saving global settings");
            if (config == null)
            {
                config = PluginConfiguration.CreateForType<SteamInputGlobalSettings>();
            }

            config.SetValue(CONFIG_KEY_LOG_LEVEL, _logLevel.ToString());
            config.SetValue(CONFIG_KEY_SHOW_LOGGING_ICON, _showLoggingIcon);
            config.SetValue(CONFIG_KEY_CONTROLLER_VDF_PATH, _controllerVdfPath ?? string.Empty);

            config.save();
            OnConfigurationChanged.Fire();
            LOGGER.LogDebug($"Saved log level: {_logLevel}, showLoggingIcon: {_showLoggingIcon}, controllerVdfPath: {_controllerVdfPath}");
        }
    }
}
