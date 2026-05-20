using UnityEngine;
using KSP.UI.Screens;
using System;
using com.github.lhervier.ksp;
using com.github.lhervier.ksp.ui.styles;

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
        private string controllerConfigNameBuffer = string.Empty;
        private CheatSheetViewModel viewModel;
        private TitleUI titleUI;
        private PhysicalZonesUI physicalZonesUI;

        // ===============================================================

        public void Initialize(CheatSheetViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public void Awake()
        {
            LOGGER.LogInfo("Awake");
            DontDestroyOnLoad(this);
        }

        public void Start() 
        {
            LOGGER.LogInfo("Start");
            GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);

            titleUI = new TitleUI(viewModel, () => OnWindowClosedFromUI());
            physicalZonesUI = new PhysicalZonesUI(viewModel);

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
            controllerConfigNameBuffer = viewModel.GetControllerConfigName();
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
            titleUI?.ZonesMenu.Close();
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
            titleUI.ZonesMenu.HandleOutsideClick(windowRect);
            titleUI.DrawTitle();
            physicalZonesUI.Draw();

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

            titleUI.ZonesMenu.DrawOverlay(windowRect.width);

            GUI.DragWindow(new Rect(0f, 0f, windowRect.width, SteamInputStyles.TitleBarHeight));
        }

        private void DrawSettings()
        {
            GUILayout.Label(ModLocalization.GetString("SteamInput_settings"), SteamInputStyles.MutedLabel);

            bool showIcon = viewModel.GetShowLoggingIcon();
            bool newShowIcon = GUILayout.Toggle(
                showIcon,
                ModLocalization.GetString("SteamInput_showLoggingIcon"),
                SteamInputStyles.Toggle
            );
            if (newShowIcon != showIcon)
            {
                viewModel.SetShowLoggingIcon(newShowIcon);
            }

            GUILayout.Space(4);
            GUILayout.Label(ModLocalization.GetString("SteamInput_controllerConfigName"), SteamInputStyles.Label);
            string newName = GUILayout.TextField(controllerConfigNameBuffer, SteamInputStyles.TextField, GUILayout.ExpandWidth(true));
            if (newName != controllerConfigNameBuffer)
            {
                controllerConfigNameBuffer = newName;
                viewModel.SetControllerConfigName(newName);
            }

            var vdfError = viewModel.getLastError();
            if (!string.IsNullOrEmpty(vdfError))
            {
                GUILayout.Label(vdfError, SteamInputStyles.ErrorLabel);
            }
        }

        private void DrawCurrentActionSet()
        {
            GUILayout.Label(ModLocalization.GetString("SteamInput_currentActionSet"), SteamInputStyles.Label);
            var actionGroupLabel = viewModel.GetActionGroupLabel();
            GUILayout.Label(actionGroupLabel, SteamInputStyles.AccentLabel);
        }

        private void DrawControllerConnected() 
        {
            GUILayout.Label(ModLocalization.GetString("SteamInput_controllerConnected"), SteamInputStyles.Label);
            bool connected = viewModel.GetGamepadConnected();
            GUILayout.Label(
                connected
                    ? ModLocalization.GetString("SteamInput_yes")
                    : ModLocalization.GetString("SteamInput_no"),
                connected ? SteamInputStyles.AccentLabel : SteamInputStyles.WarnLabel);
        }

        private void DrawActivatedContexts()
        {
            GUILayout.Label(ModLocalization.GetString("SteamInput_activatedContexts"));
            foreach (string context in viewModel.GetActivatedContexts())
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
            GUILayout.Label(ModLocalization.GetString("SteamInput_logLevel"), SteamInputStyles.Label);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(viewModel.GetLogLevel().ToString(), SteamInputStyles.MenuButton))
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
                        viewModel.SetLogLevel(level);
                        showLogLevelMenu = false;
                    }
                }
                GUILayout.EndVertical();
            }
        }
    }
}
