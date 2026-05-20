using KSP.IO;
using System;

namespace com.github.lhervier.ksp
{
    public class SteamInputGlobalSettings
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("GlobalSettings");
        private const string CONFIG_KEY_LOG_LEVEL = "SteamInput.LogLevel";
        private const string CONFIG_KEY_SHOW_LOGGING_ICON = "SteamInput.ShowLoggingIcon";
        private const string CONFIG_KEY_CONTROLLER_CONFIG_NAME = "SteamInput.ControllerConfigName";
        private static PluginConfiguration config;

        private static LogLevel _logLevel = LogLevel.Info;
        private static bool _showLoggingIcon;
        private static string _controllerConfigName = string.Empty;

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
            _controllerConfigName = config.GetValue(CONFIG_KEY_CONTROLLER_CONFIG_NAME, string.Empty);
            
            OnConfigurationChanged.Fire();
            LOGGER.LogDebug($"Loaded log level: {_logLevel}, showLoggingIcon: {_showLoggingIcon}, controllerConfigName: {_controllerConfigName}");
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
            config.SetValue(CONFIG_KEY_CONTROLLER_CONFIG_NAME, _controllerConfigName ?? string.Empty);

            config.save();
            OnConfigurationChanged.Fire();
            LOGGER.LogDebug($"Saved log level: {_logLevel}, showLoggingIcon: {_showLoggingIcon}, controllerConfigName: {_controllerConfigName}");
        }
    }
}
