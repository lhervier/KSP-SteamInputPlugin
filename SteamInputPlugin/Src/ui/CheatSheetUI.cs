using UnityEngine;
using KSP.UI.Screens;
using System;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.imgui;
using com.github.lhervier.ksp.ui.ugui;

namespace com.github.lhervier.ksp.ui
{
    public class CheatSheetUI : MonoBehaviour
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("SteamInputSettingsUI");

        private ApplicationLauncherButton button;
        private CheatSheetViewModel viewModel;
        private CheatSheetIMGUIWindow imguiWindow;
        private CheatSheetUGUIWindow uguiWindow;

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

            imguiWindow = new CheatSheetIMGUIWindow();
            imguiWindow.Initialize(viewModel, () => OnWindowClosedFromUI());
            
            uguiWindow = new CheatSheetUGUIWindow();
            uguiWindow.Initialize(viewModel, () => OnWindowClosedFromUI());

            LOGGER.LogInfo("Start: Started");
        }

        public void OnDestroy()
        {
            LOGGER.LogInfo("OnDestroy");
            GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIAppLauncherReady);
            uguiWindow?.Destroy();
            imguiWindow?.Destroy();
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
            imguiWindow?.Show();
            uguiWindow?.Show();
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
            imguiWindow?.Hide();
            uguiWindow?.Hide();
        }

        /// <summary>After closing from the in-window button — resync toolbar toggle.</summary>
        private void OnWindowClosedFromUI()
        {
            LOGGER.LogDebug("Hiding window (from UI)");
            CloseWindow();
            if (button != null)
            {
                button.SetFalse();
            }
        }

        // ===============================================================

        void OnGUI()
        {
            imguiWindow?.OnGUI();
        }
    }
}
