using UnityEngine;
using KSP.UI.Screens;
using com.github.lhervier.ksp.steaminput.ui.ugui;

namespace com.github.lhervier.ksp.steaminput.ui
{
    public class CheatSheetUI : MonoBehaviour
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("SteamInputSettingsUI");

        private ApplicationLauncherButton button;
        private CheatSheetViewModel viewModel;

        // The popup is spawned on demand: the controller only exists while the window is open.
        private ModPopupDialogBuilder popupDialogBuilder;
        private ModPopupDialogBuilder.ModPopupDialogController popupDialogController = null;

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
            popupDialogBuilder = new ModPopupDialogBuilder(viewModel);
            if( SteamInputSettings.TryGetWindowPosition(out Vector2 saved) )
            {
                popupDialogBuilder = popupDialogBuilder.Position(saved);
            }
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
            ShowInternal();
        }

        private void OnToggleOff()
        {
            LOGGER.LogDebug("Hiding window (from toolbar)");
            if (popupDialogController != null)
            {
                popupDialogController.Hide();
            }
        }

        public void WindowClosed()
        {
            LOGGER.LogDebug("Window hidden from UI");
            if (button != null)
            {
                button.SetFalse(false);
            }
        }

        public void OnWindowPositionCaptured(Vector2 position)
        {
            SteamInputSettings.SetWindowPosition(position);
        }

        // ===============================================================

        private void ShowInternal()
        {
            if (popupDialogController == null)
            {
                popupDialogController = popupDialogBuilder.Build();
                if (popupDialogController == null) return;    // Spawn failed
                popupDialogController.OnClosed.Add(WindowClosed);
                // When KSP dismisses the popup itself (Escape opens the pause menu and closes it),
                // resync as if the user had closed it: hide the rest and reset the toolbar toggle.
                popupDialogController.OnPositionCaptured.Add(OnWindowPositionCaptured);
            }
            popupDialogController.Show();
        }

        private void Dismiss()
        {
            if (popupDialogController == null) return;
            popupDialogController.OnClosed.Remove(WindowClosed);
            popupDialogController.OnPositionCaptured.Remove(OnWindowPositionCaptured);
            popupDialogController.Dismiss();
            popupDialogController = null;
        }
    }
}
