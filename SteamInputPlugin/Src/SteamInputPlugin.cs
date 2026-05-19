using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine.SceneManagement;
using com.github.lhervier.ksp.ui;
using System.IO;

namespace com.github.lhervier.ksp 
{
    [KSPAddon(KSPAddon.Startup.PSystemSpawn, true)]
    public class SteamInputPlugin : MonoBehaviour 
    {
        
        // <summary>
        //  Logger
        // </summary>
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger();
        private static SteamInputPlugin _instance;
        public static SteamInputPlugin Instance {
            get {
                return _instance;
            }
        }

        // ==================================================================================

        // <summary>
        //  Message indicating when on Steam Controller action set changes
        // </summary>
        private ScreenMessage screenMessage;

        // <summary>
        //  Connection Daemon to the steam controller
        // </summary>
        private GamepadDaemon steamInputDaemon;

        // <summary>
        //  Action Set Daemon
        // </summary>
        private ActionGroupDaemon actionGroupDaemon;
        
        // <summary>
        //  The GUI
        // </summary>
        private CheatSheetUI loggingUI;

        // <summary>
        //  Coroutine to initialize the plugin
        // </summary>
        private IEnumerator initializePluginCoroutine;

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
            LOGGER.LogDebug("Awaked");
        }

        // <summary>
        //  Start of the plugin
        // </summary>
        protected void Start() 
        {   
            LOGGER.LogInfo("Start");
            this.initializePluginCoroutine = InitializePlugin();
            StartCoroutine(this.initializePluginCoroutine);
            _instance = this;
            LOGGER.LogDebug("Started");
        }

        private IEnumerator InitializePlugin()
        {
            // Wait for the KSPSteamController to be handled
            LOGGER.LogInfo("Waiting for Squad KSPSteamController plugin");
            yield return StartCoroutine(HandleKSPSteamController());

            // Load the global settings
            SteamInputGlobalSettings.Load();

            // Create the action set daemon
            LOGGER.LogInfo("Creating Action Set Daemon");
            this.actionGroupDaemon = gameObject.AddComponent<ActionGroupDaemon>();
            this.actionGroupDaemon.OnActionGroupChanged.Add(this.OnActionGroupChanged);
            
            // Create the controller daemon
            LOGGER.LogInfo("Creating SteamInput Daemon");
            this.steamInputDaemon = gameObject.AddComponent<GamepadDaemon>();
            this.steamInputDaemon.OnGamepadConnected.Add(this.OnControllerConnected);
            this.steamInputDaemon.OnGamepadConnectedWithError.Add(this.OnControllerConnectedWithError);
            
            // Prepare screen message
            LOGGER.LogInfo("Creating Status Message");
            this.screenMessage = new ScreenMessage(
                string.Empty, 
                5f, 
                ScreenMessageStyle.UPPER_RIGHT
            );
            LOGGER.LogInfo("Status message ready");

            // Start the GUI
            LOGGER.LogInfo("Starting Logging UI");
            this.loggingUI = gameObject.AddComponent<CheatSheetUI>();
            LOGGER.LogInfo("Logging UI started");
            
            LOGGER.LogInfo("Started");
        }

        // <summary>
        //  Plugin destroyed
        // </summary>
        public void OnDestroy() 
        {
            if( this.initializePluginCoroutine != null ) {
                StopCoroutine(this.initializePluginCoroutine);
                this.initializePluginCoroutine = null;
            }
            if( this.loggingUI != null ) {
                Destroy(this.loggingUI);
                this.loggingUI = null;
            }
            
            if( this.steamInputDaemon != null ) {
                this.steamInputDaemon.OnGamepadConnected.Remove(OnControllerConnected);
                this.steamInputDaemon.OnGamepadConnectedWithError.Remove(OnControllerConnectedWithError);
                Destroy(this.steamInputDaemon);
                this.steamInputDaemon = null;
            }

            if( this.actionGroupDaemon != null ) {
                this.actionGroupDaemon.OnActionGroupChanged.Remove(OnActionGroupChanged);
                Destroy(this.actionGroupDaemon);
                this.actionGroupDaemon = null;
            }
            
            SteamInputControllerVdf.Clear();
            _instance = null;
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

        /// <summary>
        /// Called when the action group has changed
        /// </summary>
        /// <param name="actionGroup">The action group that has changed</param>
        private void OnActionGroupChanged(ActionGroup actionGroup)
        {
            LOGGER.LogInfo("Action set changed to : " + actionGroup);
            if( actionGroup == ActionGroup.None ) {
                LOGGER.LogError("Action group is None");
                return;
            }

            this.screenMessage.message = "Controller: " + actionGroup.ToString() + ".";
            ScreenMessages.PostScreenMessage(this.screenMessage);

            this.steamInputDaemon.ChangeActionGroup(actionGroup);
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
            steamInputDaemon.ChangeActionGroup(actionGroupDaemon.GetCurrentActionGroup());
            
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
