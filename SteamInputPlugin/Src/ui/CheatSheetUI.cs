using UnityEngine;
using KSP.UI.Screens;
using com.github.lhervier.ksp.steaminput.ui.ugui.titleBar;
using com.github.lhervier.ksp.steaminput.ui.ugui.body;
using com.github.lhervier.ksp.steaminput.ui.ugui.overlays;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.shared.ugui.popup;
using com.github.lhervier.ksp.shared;

namespace com.github.lhervier.ksp.steaminput.ui
{
    public class CheatSheetUI : MonoBehaviour
    {
        private static readonly ModLogger LOGGER = new ModLogger("SteamInputSettingsUI");

        private const string DIALOG_ID = "SteamInputCheatSheetUGUI";

        private ApplicationLauncherButton _toolbarButton;

        private PopupController _popupController = null;

        // ===============================================================
        // Life cycle
        // ===============================================================
        
        private CheatSheetViewModel viewModel;
        public CheatSheetUI WithViewModel(CheatSheetViewModel viewModel)
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

        // ===============================================================
        // Internal helpers
        // ===============================================================

        private void ShowInternal()
        {
            if (_popupController == null)
            {
                _popupController = new PopupBuilder<TitleBarController, BodyController, SteamInputOverlaysController>()
                    .WithPopupID(DIALOG_ID)
                    .WithTitle(ModLocalization.GetString("titleHelp"))
                    .WithIcon(SpritesTitleBar.GamepadIconSprite)
                    .WithTitleBarBuilder(
                        new TitleBarBuilder().WithViewModel(viewModel)
                    )
                    .WithContentBuilder(
                        new BodyBuilder().WithViewModel(viewModel)
                    )
                    .WithOverlayBuilder(
                        new SteamInputOverlaysBuilder().WithViewModel(viewModel)
                    )
                    .WithSize(new Vector2(SteamInputPalette.WindowWidth, SteamInputPalette.WindowHeight))
                    .Build();
                if (_popupController == null) return;    // Spawn failed
                _popupController.OnClosed.Add(WindowClosed);
            }
            _popupController.Show();
        }

        private void Dismiss()
        {
            if (_popupController == null) return;
            _popupController.OnClosed.Remove(WindowClosed);
            _popupController.Dismiss();
            _popupController = null;
        }
    }
}
