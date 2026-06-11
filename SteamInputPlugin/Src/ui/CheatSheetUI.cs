using UnityEngine;
using KSP.UI.Screens;
using com.github.lhervier.ksp.steaminput.ui.ugui;
using com.github.lhervier.ksp.shared;

namespace com.github.lhervier.ksp.steaminput.ui
{
    public class CheatSheetUI : MonoBehaviour
    {
        private static readonly ModLogger LOGGER = new ModLogger("SteamInputSettingsUI");

        private ApplicationLauncherButton _toolbarButton;

        private ModPopupController _popupController = null;

        // ===============================================================
        // Life cycle
        // ===============================================================
        
        private CheatSheetViewModel viewModel;
        public CheatSheetUI ViewModel(CheatSheetViewModel viewModel)
        {
            this.viewModel = viewModel;
            return this;
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
            LOGGER.LogInfo("Start: Started");
        }

        public void OnDestroy()
        {
            LOGGER.LogInfo("OnDestroy");
            GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIAppLauncherReady);
            Dismiss();
            LOGGER.LogInfo("OnDestroy: Destroyed");
        }

        // ===============================================================
        // Methods bounds to events
        // ===============================================================

        private void OnGUIAppLauncherReady()
        {
            LOGGER.LogInfo("Adding button to ApplicationLauncher");
            if (!ApplicationLauncher.Ready)
            {
                LOGGER.LogDebug("ApplicationLauncher not Ready");
                return;
            }
            if (_toolbarButton != null) {
                LOGGER.LogDebug("Button already added to ApplicationLauncher");
                return;
            }

            _toolbarButton = ApplicationLauncher.Instance.AddModApplication(
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
                        Constants.ModName + "/Textures/logging_icon",
                        false
                    )
            );
        }

        private void OnToggleOn()
        {
            LOGGER.LogDebug("Displaying window");
            ShowInternal();
        }

        private void OnToggleOff()
        {
            LOGGER.LogDebug("Hiding window (from toolbar)");
            if (_popupController != null)
            {
                _popupController.Hide();
            }
        }

        public void WindowClosed()
        {
            LOGGER.LogDebug("Window hidden from UI");
            if (_toolbarButton != null)
            {
                _toolbarButton.SetFalse(false);
            }
        }

        public void OnWindowPositionCaptured(Vector2 position)
        {
            SteamInputSettings.SetWindowPosition(position);
        }

        // ===============================================================
        // Internal helpers
        // ===============================================================

        private void ShowInternal()
        {
            if (_popupController == null)
            {
                ModPopupBuilder popupBuilder = new ModPopupBuilder()
                    .ViewModel(viewModel);
                if( SteamInputSettings.TryGetWindowPosition(out Vector2 saved) )
                {
                    popupBuilder = popupBuilder.Position(saved);
                }
                _popupController = popupBuilder.Build();
                if (_popupController == null) return;    // Spawn failed
                _popupController.OnClosed.Add(WindowClosed);
                // When KSP dismisses the popup itself (Escape opens the pause menu and closes it),
                // resync as if the user had closed it: hide the rest and reset the toolbar toggle.
                _popupController.OnPositionCaptured.Add(OnWindowPositionCaptured);
            }
            _popupController.Show();
        }

        private void Dismiss()
        {
            if (_popupController == null) return;
            _popupController.OnClosed.Remove(WindowClosed);
            _popupController.OnPositionCaptured.Remove(OnWindowPositionCaptured);
            _popupController.Dismiss();
            _popupController = null;
        }
    }
}
