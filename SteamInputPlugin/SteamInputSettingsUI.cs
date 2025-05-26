using UnityEngine;
using KSP.UI.Screens;
using JetBrains.Annotations;
using System;
using UnityEditor;
using System.Collections.Generic;

namespace com.github.lhervier.ksp 
{
    public class SteamInputSettingsUI : MonoBehaviour
    {
        private const int WINDOW_ID = 0x53495355; // "SISUI" ("SteamInputSettingsUI" in hex)
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("SteamInputSettingsUI");
        private static readonly Color KSPYellow = new Color(0.95f, 0.82f, 0.23f);
        private static readonly Color KSPGreen = new Color(0.5f, 1f, 0.5f);

        private ApplicationLauncherButton button;
        private bool showWindow = false;
        private Rect windowRect = new Rect(20, 20, 250, 150);
        private bool lastShowLoggingIcon;
        private bool showLogLevelMenu = false;

        // ===============================================================

        public void Awake()
        {
            LOGGER.LogInfo("Awake");
            DontDestroyOnLoad(this);
        }

        public void Start() 
        {
            LOGGER.LogInfo("Start");
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
            SteamInputGameSettings settings;
            try {
                settings = HighLogic.CurrentGame.Parameters.CustomParams<SteamInputGameSettings>();
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

            // Utilise le skin KSP si disponible
            var oldSkin = GUI.skin;
            if (HighLogic.Skin != null)
                GUI.skin = HighLogic.Skin;

            windowRect = GUILayout.Window(
                WINDOW_ID, 
                windowRect, 
                DrawWindow, 
                "SteamInput Settings",
                GUI.skin.window
            );

            GUI.skin = oldSkin;
        }

        private void DrawWindow(int windowID)
        {
            GUILayout.BeginVertical(GUI.skin.box);

            // Ajout du bouton de fermeture en haut à droite
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
            {
                showWindow = false;
            }
            GUILayout.EndHorizontal();

            GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.fontSize = 16;
            titleStyle.normal.textColor = Color.white;
            GUILayout.Label("SteamInput - Infos", titleStyle);

            GUILayout.Space(8);
            DrawCurrentActionSet();

            GUILayout.Space(8);
            DrawControllerConnected();
            
            GUILayout.Space(8);
            DrawActivatedContexts();
            
            GUILayout.Space(8);
            DrawLogLevel();

            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        private void DrawCurrentActionSet()
        {
            GUILayout.Label("Current action set:");
            GUIStyle currentActionGroupStyle = new GUIStyle(GUI.skin.label);
            currentActionGroupStyle.normal.textColor = KSPYellow;
            // currentActionGroupStyle.fontStyle = FontStyle.Bold;
            GUILayout.Label(SteamInputDaemon.Instance.CurrentActionSet, currentActionGroupStyle);
        }

        private void DrawControllerConnected() 
        {
            GUILayout.Label("Controller connected:");
            GUIStyle controllerConnectedStyle = new GUIStyle(GUI.skin.label);
            controllerConnectedStyle.normal.textColor = SteamInputDaemon.Instance.ControllerConnected ? KSPGreen : KSPYellow;
            // controllerConnectedStyle.fontStyle = FontStyle.Bold;
            GUILayout.Label(SteamInputDaemon.Instance.ControllerConnected ? "Yes" : "No", controllerConnectedStyle);
        }

        private void DrawActivatedContexts()
        {
            GUILayout.Label("Activated context(s):");
            foreach (string context in SteamInputPlugin.Instance.ActivatedContexts)
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.normal.textColor = KSPYellow;
                // style.fontStyle = FontStyle.Bold;
                string contextName;
                if( context.EndsWith("CtxDaemon") ) {
                    contextName = context.Substring(0, context.Length - "CtxDaemon".Length);
                } else {
                    contextName = context;
                }
                GUILayout.Label("- " + contextName, style);
            }
        }

        private void DrawLogLevel()
        {
            GUILayout.Label("Log Level:");
            GUILayout.BeginHorizontal();
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.normal.textColor = KSPYellow;
            // buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.fixedWidth = 100;
            if (GUILayout.Button(SteamInputGlobalSettings.GetLogLevel().ToString(), buttonStyle))
            {
                showLogLevelMenu = !showLogLevelMenu;
            }
            GUILayout.EndHorizontal();
            
            if (showLogLevelMenu)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                foreach (LogLevel level in Enum.GetValues(typeof(LogLevel)))
                {
                    if (GUILayout.Button("=> " + level.ToString(), GUILayout.Width(100)))
                    {
                        SteamInputGlobalSettings.SetLogLevel(level);
                        showLogLevelMenu = false;
                    }
                }
                GUILayout.EndVertical();
            }
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
