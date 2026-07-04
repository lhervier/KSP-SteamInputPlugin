using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using KSP.UI.Screens;
using com.github.lhervier.ksp.steaminput.ui;
using com.github.lhervier.ksp.steaminput.ui.ugui.titleBar;
using com.github.lhervier.ksp.steaminput.ui.ugui.body;
using com.github.lhervier.ksp.steaminput.ui.ugui.overlays;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.model;
using com.github.lhervier.ksp.shared;
using com.github.lhervier.ksp.shared.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui.popup;

namespace com.github.lhervier.ksp.steaminput 
{
    [KSPAddon(KSPAddon.Startup.PSystemSpawn, true)]
    public class Mod : MonoBehaviour 
    {
        // <summary>
        //  Logger
        // </summary>
        private static readonly ModLogger LOGGER = new ModLogger();

        // <summary>
        //  Persistent id of the cheat sheet popup (position + open state persistence)
        // </summary>
        private const string DIALOG_ID = "SteamInputCheatSheetUGUI";

        // ==================================================================================

        // <summary>
        //  Message indicating when on Steam Controller action set changes
        // </summary>
        private ScreenMessage _screenMessage;

        // <summary>
        //  Connection Daemon to the steam controller
        // </summary>
        private GamepadDaemon _gamepadDaemon;

        // <summary>
        //  Action Set Daemon
        // </summary>
        private ActionGroupDaemon _actionGroupDaemon;

        // <summary>
        //  Gamepad Config Daemon
        // </summary>
        private GamepadConfigDaemon _gamepadConfigDaemon;
        
        // <summary>
        //  The GUI
        // </summary>
        private CheatSheetViewModel _viewModel;

        // <summary>
        //  Toolbar button + persistent popup controller. The controller lives on this GameObject so it
        //  survives KSP destroying the window (Escape) and persists its own position and open state; we
        //  only open/close it and sync the button through OnOpenChanged.
        // </summary>
        private ApplicationLauncherButton _toolbarButton;
        private PopupController _popupController;

        // <summary>
        //  Coroutine to initialize the plugin
        // </summary>
        private IEnumerator _initializePluginCoroutine;

        // ===============================================================================
        //                      Unity initialization
        // ===============================================================================

        // <summary>
        //  Make our plugin survive between scene loading
        // </summary>
        protected void Awake()
        {
            LOGGER.LogInfo("Awake");
            DontDestroyOnLoad(this);

            // PS4 button glyphs: the game fonts cannot render them, so the localization
            // files reference them through <sprite name=...> tags instead, resolved
            // against these textures. The unicode codepoint is the glyph each sprite
            // replaces, used as a fallback when a raw character slips through.
            SpritesIcons.RegisterSprite("ps4_cross", Constants.ModName + "/Textures/ps4_cross", 0x00D7);       // ×
            SpritesIcons.RegisterSprite("ps4_circle", Constants.ModName + "/Textures/ps4_circle", 0x25CB);     // ○
            SpritesIcons.RegisterSprite("ps4_triangle", Constants.ModName + "/Textures/ps4_triangle", 0x25B3); // △
            SpritesIcons.RegisterSprite("ps4_square", Constants.ModName + "/Textures/ps4_square", 0x25A1);     // □

            LOGGER.LogDebug("Awaked");
        }

        // <summary>
        //  Start of the plugin
        // </summary>
        protected void Start() 
        {   
            LOGGER.LogInfo("Start");
            this._initializePluginCoroutine = InitializePlugin();
            StartCoroutine(this._initializePluginCoroutine);
            LOGGER.LogDebug("Started");
        }

        private IEnumerator InitializePlugin()
        {
            // Load the global settings
            SteamInputSettings.OnGlobalSettingsChanged.Add(OnLogLevelChanged);
            SteamInputSettings.Load();
            
            // Wait for the KSPSteamController to be handled
            LOGGER.LogInfo("Waiting for Squad KSPSteamController plugin");
            yield return StartCoroutine(HandleKSPSteamController());

            LOGGER.LogInfo("Waiting for Steam (SteamManager)");
            bool steamReady = false;
            yield return TryInitializeSteam(() => steamReady = true);
            if (!steamReady)
            {
                yield break;
            }
            LOGGER.LogInfo("Steam ready");
            
            // Create the action set daemon
            LOGGER.LogInfo("Creating Action Set Daemon");
            this._actionGroupDaemon = gameObject.AddComponent<ActionGroupDaemon>();
            this._actionGroupDaemon.OnActionGroupChanged.Add(this.OnActionGroupChanged);
            
            // Create the controller daemon
            LOGGER.LogInfo("Creating SteamInput Daemon");
            this._gamepadDaemon = gameObject.AddComponent<GamepadDaemon>();
            this._gamepadDaemon.OnGamepadConnected.Add(this.OnControllerConnected);
            this._gamepadDaemon.OnGamepadConnectedWithError.Add(this.OnControllerConnectedWithError);
            
            // Create the gamepad config daemon
            LOGGER.LogInfo("Creating Gamepad Config Daemon");
            this._gamepadConfigDaemon = gameObject.AddComponent<GamepadConfigDaemon>();
            
            // Prepare screen message
            LOGGER.LogInfo("Creating Status Message");
            this._screenMessage = new ScreenMessage(
                string.Empty, 
                5f, 
                ScreenMessageStyle.UPPER_RIGHT
            );
            LOGGER.LogInfo("Status message ready");

            // Create the view model
            LOGGER.LogInfo("Creating View Model");
            this._viewModel = gameObject
                .AddComponent<CheatSheetViewModel>()
                .GamepadConfigDaemon(_gamepadConfigDaemon)
                .ActionGroupDaemon(_actionGroupDaemon)
                .GamepadDaemon(_gamepadDaemon);
            
            // Start the GUI
            LOGGER.LogInfo("Starting UI");
            GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
            this._popupController = new PopupBuilder<TitleBarController, BodyController, SteamInputOverlaysController>()
                .WithHost(this.gameObject)
                .WithPopupID(DIALOG_ID)
                .WithTitle(ModLocalization.GetString("titleHelp"))
                .WithIcon(SpritesTitleBar.GamepadIconSprite)
                .WithTitleBarBuilder(new TitleBarBuilder().WithViewModel(this._viewModel))
                .WithContentBuilder(new BodyBuilder().WithViewModel(this._viewModel))
                .WithOverlayBuilder(new SteamInputOverlaysBuilder().WithViewModel(this._viewModel))
                .WithSize(new Vector2(SteamInputPalette.WindowWidth, SteamInputPalette.WindowHeight))
                .Build();
            // The controller restores its own open state in its Start (after this coroutine step returns),
            // so we only subscribe: a restored-open window then syncs the toolbar through this handler.
            if (this._popupController != null)
            {
                this._popupController.OnOpenChanged.Add(OnPopupOpenChanged);
            }
            LOGGER.LogInfo("UI started");

            // Log the detected steam environment
            bool hasPath = SteamEnvironmentDetector.TryGetSteamInstallPath(out string installPath);
            bool hasAccount = SteamEnvironmentDetector.TryGetSteamAccountId(out uint accountId);
            LOGGER.LogInfo("Steam environment: "); 
            LOGGER.LogInfo("- Install path=" + (hasPath ? installPath : "(unknown)"));
            LOGGER.LogInfo("- Account id=" + (hasAccount ? accountId.ToString() : "(unknown)"));
            LOGGER.LogInfo("- App id=" + SteamEnvironmentDetector.APP_ID);
            
            LOGGER.LogInfo("Started");
        }

        /// <summary>
        /// Waits for KSP Steamworks and calls <see cref="SteamController.Init"/>. Invokes <paramref name="onSuccess"/> only when ready.
        /// </summary>
        private IEnumerator TryInitializeSteam(System.Action onSuccess)
        {
            const float timeoutSeconds = 60f;
            float elapsed = 0f;

            while (!SteamManager.Initialized && elapsed < timeoutSeconds)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            if (!SteamManager.Initialized)
            {
                LOGGER.LogInfo("Steam not detected. Plugin will not start.");
                yield break;
            }

            LOGGER.LogInfo("Initializing Steam Controller API");
            if (!Steamworks.SteamController.Init())
            {
                LOGGER.LogError("SteamController.Init() failed. Plugin will not start.");
                yield break;
            }

            onSuccess?.Invoke();
        }

        // <summary>
        //  Plugin destroyed
        // </summary>
        public void OnDestroy() 
        {
            SteamInputSettings.Save();
            
            if( this._initializePluginCoroutine != null ) {
                StopCoroutine(this._initializePluginCoroutine);
                this._initializePluginCoroutine = null;
            }
            
            GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIAppLauncherReady);
            RemoveToolbarButton();
            // _popupController is a component on this GameObject: Unity destroys it with us, and it
            // dismisses a still-open window in its own OnDestroy. We only unsubscribe and drop the ref.
            if( this._popupController != null ) {
                this._popupController.OnOpenChanged.Remove(OnPopupOpenChanged);
                this._popupController = null;
            }

            if( this._viewModel != null ) {
                Destroy(this._viewModel);
                this._viewModel = null;
            }
            
            if( this._gamepadConfigDaemon != null ) {
                Destroy(this._gamepadConfigDaemon);
                this._gamepadConfigDaemon = null;
            }
            
            if( this._gamepadDaemon != null ) {
                this._gamepadDaemon.OnGamepadConnected.Remove(OnControllerConnected);
                this._gamepadDaemon.OnGamepadConnectedWithError.Remove(OnControllerConnectedWithError);
                Destroy(this._gamepadDaemon);
                this._gamepadDaemon = null;
            }

            if( this._actionGroupDaemon != null ) {
                this._actionGroupDaemon.OnActionGroupChanged.Remove(OnActionGroupChanged);
                Destroy(this._actionGroupDaemon);
                this._actionGroupDaemon = null;
            }

            SteamInputSettings.OnGlobalSettingsChanged.Remove(OnLogLevelChanged);

            LOGGER.LogInfo("Destroyed");
        }

        private IEnumerator HandleKSPSteamController()
        {
            // Wait for the next frame to ensure KSPSteamController has started
            yield return new WaitForEndOfFrame();
            
            LOGGER.LogInfo("Waiting for Squad KSPSteamCtrlr Plugin");
            Assembly kspSteamCtrlr = null;
            Type controllerType = null;
            MonoBehaviour controller = null;

            try {
                kspSteamCtrlr = Assembly.Load("KSPSteamCtrlr");
                if (kspSteamCtrlr == null) {
                    LOGGER.LogInfo("KSPSteamCtrlr assembly not found");
                    yield break;
                }
                
                controllerType = kspSteamCtrlr.GetType("SteamController.KSPSteamController");
                if (controllerType == null) {
                    LOGGER.LogInfo("KSPSteamController Type not found");
                    yield break;
                }
                
                controller = FindObjectOfType(controllerType) as MonoBehaviour;
                if (controller == null) {
                    LOGGER.LogInfo("KSPSteamController component not found");
                    yield break;
                }
            }
            catch (Exception ex) {
                LOGGER.LogInfo("Error loading KSPSteamCtrlr: " + ex.Message);
                yield break;
            }

            // Attendre que le controller soit actif
            while (!controller.gameObject.activeInHierarchy) {
                yield return null;
            }
            
            // Désactiver le plugin SteamController par défaut
            LOGGER.LogInfo("Desactivating Squad KSPSteamCtrlr Plugin");
            try {
                // Stop any running coroutines first
                controller.StopAllCoroutines();
                // Then disable the component
                controller.enabled = false;
                // Finally deactivate the game object
                controller.gameObject.SetActive(false);
                // And Destroy the component
                Destroy(controller);
                LOGGER.LogInfo("Squad KSPSteamCtrlr Plugin deactivated");
            }
            catch (Exception ex) {
                LOGGER.LogInfo("Error disabling Squad KSPSteamCtrlr: " + ex.Message);
            }

            // Wait for the next frame to ensure the controller is deactivated
            yield return new WaitForEndOfFrame();
        }

        // ====================================================================================
        //                      UI toolbar + window
        // ====================================================================================

        /// <summary>
        /// Add the toolbar button once the ApplicationLauncher is ready.
        /// </summary>
        private void OnGUIAppLauncherReady()
        {
            if (!ApplicationLauncher.Ready) return;
            if (_toolbarButton != null) return;

            _toolbarButton = ApplicationLauncher.Instance.AddModApplication(
                OnToggleOn,
                OnToggleOff,
                null,
                null,
                null,
                null,
                ApplicationLauncher.AppScenes.ALWAYS,
                GameDatabase.Instance.GetTexture(Constants.ModName + "/Textures/logging_icon", false)
            );

            // The launcher may become ready after the window state was restored at scene load:
            // reflect an already-open window now (false: no callback).
            if (_toolbarButton != null && _popupController != null && _popupController.IsOpen)
            {
                _toolbarButton.SetTrue(false);
            }
        }

        /// <summary>
        /// Remove the toolbar button (no-op if it was never added).
        /// </summary>
        private void RemoveToolbarButton()
        {
            if (_toolbarButton == null) return;
            try
            {
                ApplicationLauncher.Instance.RemoveModApplication(_toolbarButton);
            }
            catch (Exception e)
            {
                LOGGER.LogError("Error removing toolbar button: " + e.Message);
            }
            _toolbarButton = null;
        }

        private void OnToggleOn()
        {
            if (_popupController != null) _popupController.Show();
        }

        private void OnToggleOff()
        {
            if (_popupController != null) _popupController.Hide();
        }

        /// <summary>
        /// The window's open state changed (button, ×, Escape, or restore-at-load): keep the toolbar
        /// button pressed state in sync, notably when the change is driven by KSP or by restore rather
        /// than by a click. SetTrue/SetFalse(false): do not re-fire the toggle callbacks.
        /// </summary>
        private void OnPopupOpenChanged()
        {
            if (_toolbarButton == null) return;
            bool open = _popupController != null && _popupController.IsOpen;
            if (open)
            {
                _toolbarButton.SetTrue(false);
            }
            else
            {
                _toolbarButton.SetFalse(false);
            }
        }

        // ====================================================================================

        /// <summary>
        /// Called when the action group has changed
        /// </summary>
        /// <param name="actionGroup">The action group that has changed</param>
        private void OnActionGroupChanged(EActionGroup actionGroup)
        {
            LOGGER.LogInfo("Action set changed to : " + actionGroup);
            if( actionGroup == EActionGroup.None ) {
                LOGGER.LogError("Action group is None");
                return;
            }

            this._screenMessage.message = "Controller: " + actionGroup.ToString() + ".";
            ScreenMessages.PostScreenMessage(this._screenMessage);

            this._gamepadDaemon.ChangeActionGroup(actionGroup);
        }

        private void OnLogLevelChanged(int flags)
        {
            if( (flags & UpdatedConfiguration.LOG_LEVEL) != 0)
            {
                ModLogger.SetLogLevel(SteamInputSettings.GetLogLevel());
            }
        }

        // ==============================================================================
        //              Connection/disconnection events of controller
        // ==============================================================================
        
        // <summary>
        //  New controller connected
        // </summary>
        private void OnControllerConnected() 
        {
            LOGGER.LogInfo("New Controller connected");
            _gamepadDaemon.ChangeActionGroup(_actionGroupDaemon.GetCurrentActionGroup());
            
            // When the steam version of KSP starts, it will not see any connected Joystick
            // It's only when steam will be initialized that KSP will see the steam emulated controllers
            // But it's too late, because the game settings are already loaded.
            // So we need to reload the game settings when a new controller is connected.
            LOGGER.LogInfo("Reloading Game Settings so KSP will see the steam emulated controllers");
            GameSettings.LoadGameSettingsOnly();
        }

        /// <summary>
        /// Called when a controller is connected with an error
        /// </summary>
        /// <param name="error">The error message</param>
        private void OnControllerConnectedWithError(string error)
        {
            LOGGER.LogError("Controller connected with error: " + error);
            
            // Display a dialog box to the user
            PopupDialog.SpawnPopupDialog(
                new Vector2(0.5f, 0.5f), 
                new Vector2(0.5f, 0.5f),
                "SteamInputMod_ControllerError",
                "SteamInput Mod",
                "SteamInput is not working properly.\n\n" +
                "    Error is : " + error + "\n\n" +
                "Please check that you have copied the file:\n\n" +
                "    game_actions_220200.vdf\n\n" +
                "in the '%SteamDir%/controller_config' directory.",
                "OK",
                true,
                HighLogic.UISkin
            );
        }

    }
}
