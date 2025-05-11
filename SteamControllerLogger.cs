using UnityEngine;

namespace com.github.lhervier.ksp 
{
    public enum LogLevel
    {
        None = 0,
        Error = 1,
        Warning = 2,
        Info = 3,
        Debug = 4
    }

    public class SteamControllerLogger 
    {
        private static readonly string PREFIX = "[SteamCtrlr]";
        private readonly string additionalPrefix = "";
        private static LogLevel globalLogLevel = LogLevel.Info;
        public static void SetGlobalLogLevel(LogLevel level)
        {
            globalLogLevel = level;
        }

        public static LogLevel GetGlobalLogLevel()
        {
            return globalLogLevel;
        }

        public SteamControllerLogger() 
        {
        }
        
        public SteamControllerLogger(string additionalPrefix) : this()
        {
            this.additionalPrefix = "[" + additionalPrefix + "]";
        }

        public void Log(string message, LogLevel level) 
        {
            if (level <= globalLogLevel)
            {
                string levelPrefix;
                switch (level)
                {
                    case LogLevel.Error:
                        levelPrefix = "[ERR ]";
                        break;
                    case LogLevel.Warning:
                        levelPrefix = "[WARN]";
                        break;
                    case LogLevel.Info:
                        levelPrefix = "[INFO]";
                        break;
                    case LogLevel.Debug:
                        levelPrefix = "[DBG ]";
                        break;
                    default:
                        levelPrefix = "";
                        break;
                }
                if( level == LogLevel.Error )
                {
                    Debug.LogError(PREFIX + levelPrefix + this.additionalPrefix + " " + message);
                }
                else if( level == LogLevel.Warning )
                {
                    Debug.LogWarning(PREFIX + levelPrefix + this.additionalPrefix + " " + message);
                } else {
                    Debug.Log(PREFIX + levelPrefix + this.additionalPrefix + " " + message);
                }
            }
        }

        public void LogError(string message) => Log(message, LogLevel.Error);
        public void LogWarning(string message) => Log(message, LogLevel.Warning);
        public void LogInfo(string message) => Log(message, LogLevel.Info);
        public void LogDebug(string message) => Log(message, LogLevel.Debug);
    }
}