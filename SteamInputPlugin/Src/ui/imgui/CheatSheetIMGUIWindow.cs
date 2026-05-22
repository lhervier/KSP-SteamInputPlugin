using UnityEngine;
using KSP.UI.Screens;
using System;
using com.github.lhervier.ksp.ui.styles;

namespace com.github.lhervier.ksp.ui.imgui
{
    public class CheatSheetIMGUIWindow
    {
        private const int WINDOW_ID = 0x53495355; // "SISUI" ("SteamInputSettingsUI" in hex)
        private bool showWindow = false;
        private Rect windowRect = new Rect(20, 20, SteamInputPalette.WindowWidth, 320);
        private bool showLogLevelMenu = false;
        private string controllerConfigNameBuffer = string.Empty;
        private CheatSheetViewModel viewModel;
        private TitleUI titleUI;
        private PhysicalZonesUI physicalZonesUI;
        
        public void Initialize(
            CheatSheetViewModel viewModel,
            Action onWindowClosedFromUI
        )
        {
            this.viewModel = viewModel;
            titleUI = new TitleUI(viewModel, onWindowClosedFromUI);
            physicalZonesUI = new PhysicalZonesUI(viewModel);

            controllerConfigNameBuffer = viewModel.ControllerConfigName;
        }

        public void Destroy()
        {
        }

        public void Show()
        {
            showWindow = true;
        }

        public void Hide()
        {
            showWindow = false;
            showLogLevelMenu = false;
            titleUI?.ZonesMenu.Close();
        }

        public void OnGUI()
        {
            if (!showWindow) return;
            
            SteamInputStyles.EnsureInitialized();

            windowRect.width = SteamInputPalette.WindowWidth;
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

            GUI.DragWindow(new Rect(0f, 0f, windowRect.width, SteamInputPalette.TitleBarHeight));
        }

        private void DrawSettings()
        {
            GUILayout.Label(ModLocalization.GetString("SteamInput_settings"), SteamInputStyles.MutedLabel);

            bool showIcon = viewModel.ShowLoggingIcon;
            bool newShowIcon = GUILayout.Toggle(
                showIcon,
                ModLocalization.GetString("SteamInput_showLoggingIcon"),
                SteamInputStyles.Toggle
            );
            if (newShowIcon != showIcon)
            {
                viewModel.ShowLoggingIcon = newShowIcon;
            }

            GUILayout.Space(4);
            GUILayout.Label(ModLocalization.GetString("SteamInput_controllerConfigName"), SteamInputStyles.Label);
            string newName = GUILayout.TextField(controllerConfigNameBuffer, SteamInputStyles.TextField, GUILayout.ExpandWidth(true));
            if (newName != controllerConfigNameBuffer)
            {
                controllerConfigNameBuffer = newName;
                viewModel.ControllerConfigName = newName;
            }

            var vdfError = viewModel.LastConfigError;
            if (!string.IsNullOrEmpty(vdfError))
            {
                GUILayout.Label(vdfError, SteamInputStyles.ErrorLabel);
            }
        }

        private void DrawCurrentActionSet()
        {
            GUILayout.Label(ModLocalization.GetString("SteamInput_currentActionSet"), SteamInputStyles.Label);
            var actionGroupLabel = viewModel.ActionGroupLabel;
            GUILayout.Label(actionGroupLabel, SteamInputStyles.AccentLabel);
        }

        private void DrawControllerConnected() 
        {
            GUILayout.Label(ModLocalization.GetString("SteamInput_controllerConnected"), SteamInputStyles.Label);
            bool connected = viewModel.GamepadConnected;
            GUILayout.Label(
                connected
                    ? ModLocalization.GetString("SteamInput_yes")
                    : ModLocalization.GetString("SteamInput_no"),
                connected ? SteamInputStyles.AccentLabel : SteamInputStyles.WarnLabel);
        }

        private void DrawActivatedContexts()
        {
            GUILayout.Label(ModLocalization.GetString("SteamInput_activatedContexts"));
            foreach (string context in viewModel.ActivatedContexts)
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
            if (GUILayout.Button(viewModel.LogLevel.ToString(), SteamInputStyles.MenuButton))
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
                        viewModel.LogLevel = level;
                        showLogLevelMenu = false;
                    }
                }
                GUILayout.EndVertical();
            }
        }
    }
}