using UnityEngine;
using KSP.UI.Screens;
using System.Collections;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui;

namespace com.github.lhervier.ksp.ui
{
    public class CheatSheetUI : MonoBehaviour
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("SteamInputSettingsUI");

        private ApplicationLauncherButton button;
        private CheatSheetViewModel viewModel;
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

            uguiWindow = this.gameObject.AddComponent<CheatSheetUGUIWindow>();
            uguiWindow.Initialize(viewModel);
            
            // When KSP dismisses the popup itself (Escape opens the pause menu and closes it),
            // resync as if the user had closed it: hide the rest and reset the toolbar toggle.
            uguiWindow.OnClosed.Add(CloseWindow);
            uguiWindow.OnPositionCaptured.Add(OnWindowPositionCaptured);

            LOGGER.LogInfo("Start: Started");
        }

        public void OnDestroy()
        {
            LOGGER.LogInfo("OnDestroy");
            uguiWindow?.OnPositionCaptured.Remove(OnWindowPositionCaptured);
            uguiWindow?.OnClosed.Remove(CloseWindow);
            GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIAppLauncherReady);
            uguiWindow?.Dismiss();
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
            SteamInputGlobalSettings.SetWindowPosition(position);
        }

        // ===============================================================

        private void HideInternal()
        {
            uguiWindow?.Hide();
        }

        private void ShowInternal()
        {
            uguiWindow?.Show();
            // Restore the dragged position one frame later: KSP repositions the dialog during
            // the spawn frame, so applying it now would be overwritten.
            StartCoroutine(ApplyUguiPositionAfterLayout());
        }

        private IEnumerator ApplyUguiPositionAfterLayout()
        {
            yield return null;
            if( SteamInputGlobalSettings.TryGetWindowPosition(out Vector2 saved) ) {
                uguiWindow?.SetPosition(saved);
            }
            // Now that the layout has settled and the window sits at its final position, reveal it.
            // It was spawned hidden (alpha 0) to avoid flickering at the default spawn position.
            uguiWindow?.Reveal();
        }
    }
}
