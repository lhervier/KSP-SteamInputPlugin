using UnityEngine;
using KSP.UI.Screens;
using com.github.lhervier.ksp.ui.ugui;

namespace com.github.lhervier.ksp.ui
{
    public class CheatSheetUI : MonoBehaviour
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("SteamInputSettingsUI");

        private ApplicationLauncherButton button;
        private CheatSheetViewModel viewModel;

        // The popup is spawned on demand: the controller only exists while the window is open.
        private PopupDialogBuilder popupDialogBuilder;
        private PopupDialogBuilder.PopupDialogController popupDialogController = null;

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
            popupDialogBuilder = new PopupDialogBuilder(viewModel);
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
            HideInternal();
        }

        public void CloseWindow()
        {
            LOGGER.LogDebug("Hiding window (from UI)");
            HideInternal();
            if (button != null)
            {
                button.SetFalse();
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
                popupDialogController = popupDialogBuilder.Create();
                if (popupDialogController == null) return;    // Spawn failed
                // When KSP dismisses the popup itself (Escape opens the pause menu and closes it),
                // resync as if the user had closed it: hide the rest and reset the toolbar toggle.
                popupDialogController.OnClosed.Add(CloseWindow);
                popupDialogController.OnPositionCaptured.Add(OnWindowPositionCaptured);
            }
            popupDialogController.Show();
        }

        private void HideInternal()
        {
            if (popupDialogController == null) return;
            popupDialogController.Hide();
        }

        private void Dismiss()
        {
            if (popupDialogController == null) return;
            popupDialogController.OnClosed.Remove(CloseWindow);
            popupDialogController.OnPositionCaptured.Remove(OnWindowPositionCaptured);
            popupDialogController.Dismiss();
            popupDialogController = null;
        }
    }
}
