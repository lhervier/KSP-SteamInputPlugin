using UnityEngine;
using KSP.UI.Screens;
using JetBrains.Annotations;
using System;

namespace com.github.lhervier.ksp 
{
    public class LoggingUI : MonoBehaviour
    {
        private const int WINDOW_ID = 0x4C4F4747; // "LOGG" en hexadécimal
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("LoggingUI");
        private ApplicationLauncherButton button;
        private bool showWindow = false;
        private Rect windowRect = new Rect(20, 20, 200, 150);
        private LogLevel currentLogLevel;
        private bool lastShowLoggingIcon;

        // ===============================================================

        public void Awake()
        {
            LOGGER.LogInfo("Awake");
            DontDestroyOnLoad(this);
        }

        public void Start() 
        {
            LOGGER.LogInfo("Start");
            currentLogLevel = SteamInputLogger.GetGlobalLogLevel();
            GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
            lastShowLoggingIcon = false;
            LOGGER.LogInfo("Start: Started");
        }

        public void OnDestroy()
        {
            LOGGER.LogInfo("OnDestroy");
            GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIAppLauncherReady);
            RemoveButton();
            LOGGER.LogInfo("OnDestroy: Destroyed");
        }

        // ===============================================================

        private bool ShouldDisplayLoggingIcon()
        {
            // Outside of flight, trackstation, spacecenter or editor
            // => Display button
            if ( 
                HighLogic.LoadedScene != GameScenes.FLIGHT &&
                HighLogic.LoadedScene != GameScenes.TRACKSTATION && 
                HighLogic.LoadedScene != GameScenes.SPACECENTER &&
                HighLogic.LoadedScene != GameScenes.EDITOR
            ) {
                return true;
            }
            
            // No current game or parameters for the current game
            // => Display button
            if( HighLogic.CurrentGame == null) return true;
            if( HighLogic.CurrentGame.Parameters == null) return true;
            
            // Get custom parameters for the current game
            // => Display button depending on the value of showLoggingIcon in the parameters
            SteamInputSettings settings;
            try {
                settings = HighLogic.CurrentGame.Parameters.CustomParams<SteamInputSettings>();
            } catch {
                LOGGER.LogError("Error getting custom parameters => Displaying button");
                return true;
            }
            if( settings == null) {
                LOGGER.LogDebug("Getting null custom parameters => Displaying button");
                return true;
            }
            
            return settings.showLoggingIcon;
        }

        private void RemoveButton()
        {
            LOGGER.LogInfo("Removing button from ApplicationLauncher");
            if (!ApplicationLauncher.Ready)
            {
                LOGGER.LogDebug("ApplicationLauncher not Ready");
                return;
            }
            if (button == null) {
                LOGGER.LogDebug("Button was not added to ApplicationLauncher");
                return;
            }
            
            ApplicationLauncher.Instance.RemoveModApplication(button);
            button = null;
        }

        private void AddButton()
        {
            LOGGER.LogInfo("Adding button to ApplicationLauncher");
            if (!ApplicationLauncher.Ready)
            {
                LOGGER.LogDebug("ApplicationLauncher not Ready");
                return;
            }
            if (button != null) {
                LOGGER.LogDebug("Button already added to ApplicationLauncher");
                return;
            }

            button = ApplicationLauncher.Instance.AddModApplication(
                () => { 
                    LOGGER.LogDebug("Displaying window"); 
                    showWindow = true; 
                },
                () => { 
                    LOGGER.LogDebug("Hiding window"); 
                    showWindow = false; 
                },
                null,
                null,
                null,
                null,
                ApplicationLauncher.AppScenes.ALWAYS,
                GameDatabase
                    .Instance
                    .GetTexture(
                        "SteamInput/Textures/logging_icon", 
                        false
                    )
            );
        }

        private void SetLogLevel(LogLevel level)
        {
            LOGGER.LogDebug($"Setting log level to {level}");
            currentLogLevel = level;
            SteamInputLogger.SetGlobalLogLevel(level);
            LOGGER.LogInfo($"Log level set to {level}");
        }

        // ===============================================================

        private void OnGUIAppLauncherReady()
        {
            LOGGER.LogDebug("=> OnGUIAppLauncherReady");
            if( ShouldDisplayLoggingIcon()) {
                AddButton();
            } else {
                RemoveButton();
            }
        }

        void OnGUI()
        {
            if (!showWindow) return;
            
            windowRect = GUILayout.Window(
                WINDOW_ID, 
                windowRect, 
                DrawWindow, 
                "SteamInput Logging Settings"
            );
        }

        private void DrawWindow(int windowID)
        {
            GUILayout.BeginVertical();

            GUILayout.Label("Current action set: " + SteamInputDaemon.Instance.CurrentActionSet);

            GUILayout.Label("Log Level:");
            foreach (LogLevel level in Enum.GetValues(typeof(LogLevel)))
            {
                GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                if( level == currentLogLevel ) 
                {
                    buttonStyle.normal.textColor = Color.yellow;
                }
                if (GUILayout.Button(level.ToString(), buttonStyle))
                {
                    SetLogLevel(level);
                }
            }
            
            GUILayout.Space(10);
            GUILayout.Label("Current: " + currentLogLevel.ToString());

            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        void Update()
        {
            bool currentShowLoggingIcon = ShouldDisplayLoggingIcon();
            if( currentShowLoggingIcon == lastShowLoggingIcon ) return;
            LOGGER.LogDebug($"showLoggingIcon changed: {lastShowLoggingIcon} -> {currentShowLoggingIcon}");
            if( currentShowLoggingIcon ) {
                AddButton();
            } else {
                RemoveButton();
            }
            lastShowLoggingIcon = currentShowLoggingIcon;
        }
    }
}
