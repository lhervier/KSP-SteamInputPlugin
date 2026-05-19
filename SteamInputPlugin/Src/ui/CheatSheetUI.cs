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
        private bool showLogLevelMenu = false;
        private string controllerVdfPathBuffer = string.Empty;
        private CheatSheetViewModel viewModel;
        private TitleUI titleUI;

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

            viewModel = new CheatSheetViewModel();

            titleUI = new TitleUI(viewModel, () => OnWindowClosedFromUI());

            LOGGER.LogInfo("Start: Started");
        }

        public void OnDestroy()
        {
            LOGGER.LogInfo("OnDestroy");
            GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIAppLauncherReady);
            LOGGER.LogInfo("OnDestroy: Destroyed");
        }

        // ===============================================================

        private void OnGUIAppLauncherReady()
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

        // ===============================================================

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

        // ===============================================================

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
            titleUI.DrawTitle();

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
            var actionSetName = ActionGroupDaemon.Instance.GetCurrentActionGroup().ToString();
            var actionSetLabel = actionSetName != null
                ? SteamInputControllerVdf.GetActionSetTitle(actionSetName)
                : "—";
            GUILayout.Label(actionSetLabel, SteamInputStyles.AccentLabel);
        }

        private void DrawControllerConnected() 
        {
            GUILayout.Label("Controller connected:", SteamInputStyles.Label);
            bool connected = GamepadDaemon.Instance.GamepadConnected;
            GUILayout.Label(connected ? "Yes" : "No", connected ? SteamInputStyles.AccentLabel : SteamInputStyles.WarnLabel);
        }

        private void DrawActivatedContexts()
        {
            GUILayout.Label("Activated context(s):");
            foreach (string context in ActionGroupDaemon.Instance.ActivatedContexts)
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
    }
}
