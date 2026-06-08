using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Steamworks;

namespace com.github.lhervier.ksp.steaminput
{
    /// <summary>
    /// Detects Steam client install path and current user account id (SteamID3 folder name in userdata / controller configs).
    /// Uses the same Steamworks surface as KSP (no Steam Input API).
    /// </summary>
    public static class SteamEnvironmentDetector
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("SteamEnvironmentDetector");
        public const string APP_ID = "220200"; // KSP app id
        private const uint AppInstallDirBufferSize = 4096;
        
        /// <summary>
        /// Resolves the Steam client root directory (contains steamapps, userdata, etc.).
        /// </summary>
        public static bool TryGetSteamInstallPath(out string steamInstallPath)
        {
            steamInstallPath = null;

            // Registry is the best place to find Steam install dir. Will also work on proton.
            if (TryGetSteamRootFromRegistry(out steamInstallPath))
            {
                LOGGER.LogInfo("Steam environment: found in registry");
                return true;
            }

            // If native KSP on Linux, the Steam install dir is in the user's home folder.
            if (TryGetSteamRootFromLinuxUserSteam(out steamInstallPath))
            {
                LOGGER.LogInfo("Steam environment: found in Linux user Steam folder");
                return true;
            }

            // KSP has detected it's own Steam app folder for workshop content.
            // Will only work if KSP is installed in the main Library.
            if (TryGetSteamRootFromKspSteamAppFolder(out steamInstallPath))
            {
                LOGGER.LogInfo("Steam environment: found in KSP Steam app folder");
                return true;
            }

            // Try using Steamworks API to find the Steam install dir.
            // If KspSteamAppFolder is not set, this will probably not work...
            if (TryGetSteamRootFromGameInstallDir(out steamInstallPath))
            {
                LOGGER.LogInfo("Steam environment: found in game install dir");
                return true;
            }

            LOGGER.LogInfo("Steam environment: not found");
            return false;
        }

        /// <summary>
        /// Account id used in paths such as Steam Controller Configs / userdata (e.g. 27319809).
        /// </summary>
        public static bool TryGetSteamAccountId(out uint accountId)
        {
            accountId = 0;
            if (!SteamManager.Initialized)
            {
                return false;
            }

            try
            {
                accountId = SteamUser.GetSteamID().GetAccountID().m_AccountID;
                return accountId != 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // ===============================================================
        // Private methods
        // ===============================================================

        private static bool TryGetSteamRootFromGameInstallDir(out string steamInstallPath)
        {
            steamInstallPath = null;
            if (!SteamManager.Initialized)
            {
                return false;
            }

            try
            {
                string gameInstallDir;
                uint pathLength = SteamApps.GetAppInstallDir(SteamManager.AppID, out gameInstallDir, AppInstallDirBufferSize);
                if (pathLength == 0 || string.IsNullOrEmpty(gameInstallDir))
                {
                    return false;
                }

                return TryDeriveSteamRootFromGameFolder(gameInstallDir, out steamInstallPath);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool TryGetSteamRootFromKspSteamAppFolder(out string steamInstallPath)
        {
            steamInstallPath = null;
            string gameFolder = SteamManager.KSPSteamAppFolder;
            if (string.IsNullOrEmpty(gameFolder))
            {
                return false;
            }

            return TryDeriveSteamRootFromGameFolder(gameFolder, out steamInstallPath);
        }

        /// <summary>
        /// .../steamapps/common/&lt;Game&gt; -> .../Steam (three levels up).
        /// </summary>
        private static bool TryDeriveSteamRootFromGameFolder(string gameInstallDir, out string steamInstallPath)
        {
            steamInstallPath = null;
            if (string.IsNullOrEmpty(gameInstallDir))
            {
                return false;
            }

            steamInstallPath = Path.GetFullPath(Path.Combine(gameInstallDir, "..", "..", ".."));
            if (Directory.Exists(Path.Combine(steamInstallPath, "steamapps")))
            {
                return true;
            }

            steamInstallPath = null;
            return false;
        }

        /// <summary>
        /// Linux native Steam layout (~/.steam/steam, ~/.local/share/Steam). No-op when paths are absent (e.g. Windows).
        /// </summary>
        private static bool TryGetSteamRootFromLinuxUserSteam(out string steamInstallPath)
        {
            steamInstallPath = null;
            if (!IsLinuxPlatform())
            {
                return false;
            }

            string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (string.IsNullOrEmpty(home))
            {
                return false;
            }

            string[] candidates =
            {
                Path.Combine(home, ".steam", "steam"),
                Path.Combine(home, ".steam", "root"),
                Path.Combine(home, ".local", "share", "Steam"),
            };

            foreach (string candidate in candidates)
            {
                if (!Directory.Exists(candidate))
                {
                    continue;
                }

                string fullPath = Path.GetFullPath(candidate);
                if (Directory.Exists(Path.Combine(fullPath, "steamapps")))
                {
                    steamInstallPath = fullPath;
                    return true;
                }
            }

            return false;
        }

        private static bool IsLinuxPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return true;
            }

            // Mono (KSP): Linux is often reported as 128, not PlatformID.Unix (4). macOS is 6 — exclude it.
            int platform = (int)Environment.OSVersion.Platform;
            return platform == (int)PlatformID.Unix || platform == 128;
        }

        private static bool TryGetSteamRootFromRegistry(out string steamInstallPath)
        {
            steamInstallPath = null;
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam"))
                {
                    var path = key?.GetValue("SteamPath") as string;
                    if (string.IsNullOrEmpty(path))
                    {
                        return false;
                    }

                    steamInstallPath = Path.GetFullPath(path.TrimEnd('\\', '/'));
                    return Directory.Exists(Path.Combine(steamInstallPath, "steamapps"));
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
