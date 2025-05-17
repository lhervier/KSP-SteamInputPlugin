using KSP.Localization;
using KSP.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

namespace com.github.lhervier.ksp
{
    public class SteamInputGameSettings : GameParameters.CustomParameterNode
    {
        [GameParameters.CustomParameterUI("Show Logging Icon", toolTip = "Show the logging icon in the action bar", autoPersistance = true)]
        public bool showLoggingIcon = false;

        public override string Title => "Steam Input";
        public override string Section => "Steam Input";
        public override string DisplaySection => "Steam Input";
        public override int SectionOrder => 1;
        public override bool HasPresets => false;
        public override GameParameters.GameMode GameMode => GameParameters.GameMode.ANY;

        public static SteamInputGameSettings Instance => HighLogic.CurrentGame?.Parameters?.CustomParams<SteamInputGameSettings>();
    }

    public class SteamInputGlobalSettings
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("GlobalSettings");
        private static PluginConfiguration config;
        
        /// <summary>
        /// Log level
        /// </summary>
        private static LogLevel _logLevel = LogLevel.Info;
        public static LogLevel LogLevel
        {
            get { return _logLevel; }
            set
            {
                _logLevel = value;
                Save();
            }
        }

        public static void Load()
        {
            LOGGER.LogDebug("Loading global settings");
            
            // Load the config
            // ================
            config = PluginConfiguration.CreateForType<SteamInputGlobalSettings>();
            config.load();
            
            // Load the log level
            // ==================
            _logLevel = config.GetValue("LogLevel", LogLevel.Info);
            LOGGER.LogDebug($"Loaded log level: {_logLevel}");
        }

        public static void Save()
        {
            LOGGER.LogDebug("Saving global settings");
            if (config == null)
            {
                config = PluginConfiguration.CreateForType<SteamInputGlobalSettings>();
            }
            
            // The log level
            config.SetValue("LogLevel", _logLevel);

            // Save the config
            config.save();
            LOGGER.LogDebug($"Saved log level: {_logLevel}");
        }
    }
} 