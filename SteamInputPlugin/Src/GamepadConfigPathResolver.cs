using System;
using System.IO;
using com.github.lhervier.ksp.shared;

namespace com.github.lhervier.ksp.steaminput
{
    /// <summary>
    /// Builds the absolute path to a Gamepad Config VDF from a config base name
    /// (e.g. ksp_steaminput_steamcontroller_v2_french → ..._101.vdf with highest numeric suffix).
    /// </summary>
    public static class GamepadConfigPathResolver
    {
        private static readonly ModLogger LOGGER = new ModLogger("GamepadConfigPathResolver");

        private const string ControllerConfigsFolder = "Steam Controller Configs";

        /// <summary>
        /// Resolves the VDF file path for <paramref name="configName"/>, or null if <paramref name="configName"/> is empty.
        /// </summary>
        public static bool TryResolve(string configName, out string vdfPath, out string error)
        {
            vdfPath = null;
            error = null;

            configName = NormalizeConfigName(configName);
            if (string.IsNullOrEmpty(configName))
            {
                error = "Config name is empty.";
                LOGGER.LogError(error);
                return false;
            }

            if (!TryGetConfigDirectory(out string configDir, out error))
            {
                LOGGER.LogError(error);
                return false;
            }

            string bestPath = null;
            int bestSuffix = -1;

            foreach (string file in Directory.GetFiles(configDir, "*.vdf"))
            {
                string baseName = Path.GetFileNameWithoutExtension(file);
                if (!baseName.StartsWith(configName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if( baseName == configName )
                {
                    baseName += "_0";
                }
                string suffixPart = baseName.Substring(configName.Length + 1);
                if (!int.TryParse(suffixPart, out int suffix) || suffix <= bestSuffix)
                {
                    continue;
                }

                bestSuffix = suffix;
                bestPath = file;
            }

            if (bestPath == null)
            {
                error = "No VDF found for config \"" + configName + "\" in " + configDir
                    + " (expected " + configName + "[_<number>].vdf).";
                LOGGER.LogError(error);
                return false;
            }

            vdfPath = Path.GetFullPath(bestPath);
            LOGGER.LogDebug("Resolved config \"" + configName + "\" to " + vdfPath);
            return true;
        }

        /// <summary>
        /// Resolves the directory holding the Steam controller configs for this account/app,
        /// or false (with an error) if Steam, the account, or the directory is unavailable.
        /// </summary>
        public static bool TryGetConfigDirectory(out string configDir, out string error)
        {
            configDir = null;
            error = null;

            if (!SteamEnvironmentDetector.TryGetSteamInstallPath(out string steamRoot))
            {
                error = "Steam install path not found.";
                return false;
            }

            if (!SteamEnvironmentDetector.TryGetSteamAccountId(out uint accountId))
            {
                error = "Steam account id not available.";
                return false;
            }

            string dir = Path.Combine(
                steamRoot,
                "steamapps",
                "common",
                ControllerConfigsFolder,
                accountId.ToString(),
                "config",
                SteamEnvironmentDetector.APP_ID);

            if (!Directory.Exists(dir))
            {
                error = "Controller config directory not found: " + dir;
                return false;
            }

            configDir = dir;
            return true;
        }

        /// <summary>
        /// Keeps only the config base name (no path, .vdf, or _NNN version suffix).
        /// </summary>
        private static string NormalizeConfigName(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }
            return Path.GetFileName(input.Trim());
        }
    }
}
