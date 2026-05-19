using UnityEngine;
using KSP.UI.Screens;
using System;

namespace com.github.lhervier.ksp.ui
{
    public class CheatSheetUI : MonoBehaviour
    {
        private const int WINDOW_ID = 0x53495355; // "SISUI" ("SteamInputSettingsUI" in hex)
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("SteamInputSettingsUI");

        private ApplicationLauncherButton button;
        private bool showWindow = false;
        private Rect windowRect = new Rect(20, 20, SteamInputStyles.WindowWidth, 320);
        private bool lastShowLoggingIcon;
        private bool showLogLevelMenu = false;
        private string controllerVdfPathBuffer = string.Empty;

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
            lastShowLoggingIcon = ShouldDisplayLoggingIcon();
            controllerVdfPathBuffer = SteamInputGlobalSettings.GetControllerVdfPath();
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

            return SteamInputGlobalSettings.GetShowLoggingIcon();
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
                OnToggleOn,
                OnToggleOff,
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

        private void OnToggleOn()
        {
            LOGGER.LogDebug("Displaying window");
            controllerVdfPathBuffer = SteamInputGlobalSettings.GetControllerVdfPath();
            showWindow = true;
        }

        /// <summary>Toolbar off callback — visibility only, like VesselBookmark OnToggleOff.</summary>
        private void OnToggleOff()
        {
            LOGGER.LogDebug("Hiding window (toolbar)");
            CloseWindow();
        }

        private void CloseWindow()
        {
            showWindow = false;
            showLogLevelMenu = false;
        }

        /// <summary>After closing from the in-window button — resync toolbar toggle.</summary>
        private void OnWindowClosedFromUI()
        {
            CloseWindow();
            if (button != null)
            {
                button.SetFalse();
            }
        }

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

            SteamInputStyles.EnsureInitialized();

            windowRect.width = SteamInputStyles.WindowWidth;
            windowRect = GUILayout.Window(
                WINDOW_ID, 
                windowRect, 
                DrawWindow, 
                string.Empty,
                SteamInputStyles.Window
            );
        }

        private void DrawWindow(int windowID)
        {
            DrawTitleBar();

            GUILayout.BeginVertical(SteamInputStyles.Body);
            DrawSettings();
            GUILayout.Space(6);
            DrawCurrentActionSet();
            GUILayout.Space(6);
            DrawControllerConnected();
            GUILayout.Space(6);
            DrawActivatedContexts();
            GUILayout.Space(6);
            DrawLogLevel();
            GUILayout.EndVertical();

            GUI.DragWindow(new Rect(0f, 0f, windowRect.width, SteamInputStyles.TitleBarHeight));
        }

        private void DrawTitleBar()
        {
            GUILayout.BeginHorizontal(SteamInputStyles.HeaderBar, GUILayout.Height(SteamInputStyles.TitleBarHeight));
            GUILayout.Label("AIDE MANETTE", SteamInputStyles.Title, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("×", SteamInputStyles.CloseButton))
            {
                OnWindowClosedFromUI();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawSettings()
        {
            GUILayout.Label("Settings", SteamInputStyles.MutedLabel);

            bool showIcon = SteamInputGlobalSettings.GetShowLoggingIcon();
            bool newShowIcon = GUILayout.Toggle(
                showIcon,
                "Show logging icon during flight / VAB / KSC / tracking",
                SteamInputStyles.Toggle
            );
            if (newShowIcon != showIcon)
            {
                SteamInputGlobalSettings.SetShowLoggingIcon(newShowIcon);
            }

            GUILayout.Space(4);
            GUILayout.Label("Controller VDF path (absolute):", SteamInputStyles.Label);
            string newPath = GUILayout.TextField(controllerVdfPathBuffer, SteamInputStyles.TextField, GUILayout.ExpandWidth(true));
            if (newPath != controllerVdfPathBuffer)
            {
                controllerVdfPathBuffer = newPath;
                SteamInputGlobalSettings.SetControllerVdfPath(newPath);
                SteamInputControllerVdf.Reload();
            }

            var vdfError = SteamInputControllerVdf.LastError;
            if (!string.IsNullOrEmpty(vdfError))
            {
                GUILayout.Label(vdfError, SteamInputStyles.ErrorLabel);
            }
        }

        private void DrawCurrentActionSet()
        {
            GUILayout.Label("Current action set:", SteamInputStyles.Label);
            var actionSetName = SteamInputDaemon.Instance.CurrentActionSet;
            var actionSetLabel = actionSetName != null
                ? SteamInputControllerVdf.GetActionSetTitle(actionSetName)
                : "—";
            GUILayout.Label(actionSetLabel, SteamInputStyles.AccentLabel);
        }

        private void DrawControllerConnected() 
        {
            GUILayout.Label("Controller connected:", SteamInputStyles.Label);
            bool connected = SteamInputDaemon.Instance.ControllerConnected;
            GUILayout.Label(connected ? "Yes" : "No", connected ? SteamInputStyles.AccentLabel : SteamInputStyles.WarnLabel);
        }

        private void DrawActivatedContexts()
        {
            GUILayout.Label("Activated context(s):");
            foreach (string context in SteamInputPlugin.Instance.ActivatedContexts)
            {
                string contextName;
                if( context.EndsWith("CtxDaemon") ) {
                    contextName = context.Substring(0, context.Length - "CtxDaemon".Length);
                } else {
                    contextName = context;
                }
                GUILayout.Label("- " + contextName, SteamInputStyles.WarnLabel);
            }
        }

        private void DrawLogLevel()
        {
            GUILayout.Label("Log Level:", SteamInputStyles.Label);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(SteamInputGlobalSettings.GetLogLevel().ToString(), SteamInputStyles.MenuButton))
            {
                showLogLevelMenu = !showLogLevelMenu;
            }
            GUILayout.EndHorizontal();
            
            if (showLogLevelMenu)
            {
                GUILayout.BeginVertical(SteamInputStyles.MenuBox);
                foreach (LogLevel level in Enum.GetValues(typeof(LogLevel)))
                {
                    if (GUILayout.Button("=> " + level, SteamInputStyles.MenuButton))
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
