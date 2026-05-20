using System;
using KSP.Localization;

namespace com.github.lhervier.ksp
{
    /// <summary>
    /// Localization for the Steam Input mod (KSP Localizer).
    /// </summary>
    public static class ModLocalization
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("ModLocalization");

        public static string GetString(string key, params object[] args)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }
            try
            {
                return Localizer.Format($"#LOC_{key}", args);
            }
            catch (Exception e)
            {
                LOGGER.LogWarning($"Error formatting localization string '{key}': {e.Message}");
                return $"[{key}]";
            }
        }
    }
}
