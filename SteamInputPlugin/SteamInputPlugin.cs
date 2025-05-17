using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine.SceneManagement;
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
        private static readonly SteamInputLogger LOGGER_CONTEXT = new SteamInputLogger("Contexts");
        private static SteamInputPlugin _instance;
        public static SteamInputPlugin Instance {
            get {
                return _instance;
            }
        }

        // <summary>
        //  Delay before applying an action set (in frames)
        // </summary>
        private static readonly int DELAY = 10;

        // <summary>
        //  The default action group
        // </summary>
        private static readonly ActionGroup DEFAULT_ACTION_GROUP = ActionGroup.MenuControls;

        // ==================================================================================

        // <summary>
        //  The daemons
        // </summary>
        private readonly List<BaseContextDaemon> contextDaemons = new List<BaseContextDaemon>();

        // <summary>
        //  The active contexts. Idealy, there should be only one active context.
        //  But some daemons will deactivate before the next one activates.
        //  And some daemons will activate before the previous one deactivates.
        //  So we can have zero or 2 active contexts at the same time.
        //  More than 2 active contexts should never happen.
        // </summary>
        private readonly List<BaseContextDaemon> activecontexts = new List<BaseContextDaemon>();
        public List<String> ActivatedContexts {
            get {
                return this.activecontexts.Select(c => c.GetType().Name).ToList();
            }
        }

        // <summary>
        //  Message indicating when on Steam Controller action set changes
        // </summary>
        private ScreenMessage screenMessage;

        // <summary>
        //  Previous action group (so we don't display the message when the value has not changed)
        // </summary>
        private ActionGroup prevActionGroup;

        // <summary>
        //  Connection Daemon to the steam controller
        // </summary>
        private SteamInputDaemon steamControllerDaemon;
        
        // <summary>
        //  Delayed Action daemon
        // </summary>
        private DelayedActionDaemon delayedActionDaemon;

        // <summary>
        //  The action group to set when triggering a delayed action
        // </summary>
        private ActionGroup actionGroupToSet;

        // <summary>
        //  The GUI
        // </summary>
        private SteamInputSettingsUI loggingUI;

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

            // Start the coroutine to handle the KSPSteamController
            StartCoroutine(InitializePlugin());
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

            // Create the controller daemon
            LOGGER.LogInfo("Creating SteamInput Daemon");
            this.steamControllerDaemon = gameObject.AddComponent<SteamInputDaemon>();
            LOGGER.LogInfo("SteamInput Daemon attached");
            this.steamControllerDaemon.OnControllerConnected.Add(this.OnControllerConnected);
            this.steamControllerDaemon.OnControllerDisconnected.Add(this.OnControllerDisconnected);
            LOGGER.LogInfo("Controller Events attached");
            if( this.steamControllerDaemon.ControllerConnected ) 
            {
                LOGGER.LogInfo("Controller already connected at startup");
                this.OnControllerConnected();
            }

            // Create the delayed action daemon
            LOGGER.LogInfo("Creating Delayed Actions Daemon");
            this.delayedActionDaemon = gameObject.AddComponent<DelayedActionDaemon>();
            LOGGER.LogInfo("Delayed Actions Daemon attached");
            this.actionGroupToSet = ActionGroup.None;
            this.prevActionGroup = ActionGroup.None;
            
            // Prepare screen message
            LOGGER.LogInfo("Creating Status Message");
            this.screenMessage = new ScreenMessage(
                string.Empty, 
                5f, 
                ScreenMessageStyle.UPPER_RIGHT
            );
            LOGGER.LogInfo("Status message ready");

            // Get all the daemons and attach them to the plugin
            LOGGER.LogInfo("Loading Context Daemons");
            this.LoadContextDaemons();
            this.activecontexts.Clear();
            LOGGER.LogInfo("Context Daemons loaded");

            LOGGER_CONTEXT.LogInfo("Attaching Context Daemons :");
            foreach(BaseContextDaemon daemon in this.contextDaemons) 
            {
                daemon.OnEnterContext().Add(this.OnEnterContext);
                daemon.OnExitContext().Add(this.OnExitContext);
                LOGGER_CONTEXT.LogInfo("- " + daemon.GetType().Name);
            }
            LOGGER_CONTEXT.LogInfo("Context Daemons attached : " + this.contextDaemons.Count);
            this.LogDaemons();

            // Start the GUI
            LOGGER.LogInfo("Starting Logging UI");
            this.loggingUI = gameObject.AddComponent<SteamInputSettingsUI>();
            LOGGER.LogInfo("Logging UI started");

            LOGGER.LogInfo("Started");
        }

        // <summary>
        //  Plugin destroyed
        // </summary>
        public void OnDestroy() 
        {
            Destroy(this.loggingUI);

            this.steamControllerDaemon.OnControllerDisconnected.Remove(OnControllerDisconnected);
            this.steamControllerDaemon.OnControllerConnected.Remove(OnControllerConnected);
            Destroy(this.delayedActionDaemon);
            Destroy(this.steamControllerDaemon);
            Destroy(this.loggingUI);
            
            foreach(BaseContextDaemon daemon in this.contextDaemons) 
            {
                daemon.OnEnterContext().Remove(this.OnEnterContext);
                daemon.OnExitContext().Remove(this.OnExitContext);
                Destroy((MonoBehaviour) daemon);
            }
            this.contextDaemons.Clear();
            this.activecontexts.Clear();
            _instance = null;
            LOGGER.LogInfo("Destroyed");
        }

        // <summary>
        //  Load the context daemons
        // </summary>
        private void LoadContextDaemons()
        {
            this.contextDaemons.Clear();
            
            // Get all types that implement ControllerContextDaemon
            var daemonTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(BaseContextDaemon).IsAssignableFrom(t));

            // Add each daemon component to the GameObject
            foreach (var type in daemonTypes)
            {
                BaseContextDaemon component = gameObject.AddComponent(type) as BaseContextDaemon;
                this.contextDaemons.Add(component);
            }
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

        // <summary>
        //  When a context is activated
        // </summary>
        public void OnEnterContext(BaseContextDaemon daemon)
        {
            LOGGER_CONTEXT.LogDebug("OnEnterContext : " + daemon.GetType().Name);
            this.activecontexts.Add(daemon);
            this.LogKSPContext();
            this.LogDaemons();
            this.UpdateActionGroup();
        }

        // <summary>
        //  When a context is deactivated
        // </summary>
        public void OnExitContext(BaseContextDaemon daemon)
        {
            LOGGER_CONTEXT.LogDebug("OnExitContext : " + daemon.GetType().Name);
            this.activecontexts.Remove(daemon);
            this.LogKSPContext();
            this.LogDaemons();
            this.UpdateActionGroup();
        }

        public void LogDaemons()
        {
            if( this.activecontexts.Count == 0 ) {
                LOGGER_CONTEXT.LogDebug("No active daemons contexts");
            } else if( this.activecontexts.Count == 1 ) {
                LOGGER_CONTEXT.LogDebug("Active daemon context: " + this.activecontexts[0].GetType().Name);
            } else {
                LOGGER_CONTEXT.LogDebug("Active daemons contexts: " + this.activecontexts.Count);
                foreach( BaseContextDaemon daemon in this.activecontexts ) {
                    LOGGER_CONTEXT.LogDebug("- " + daemon.GetType().Name);
                }
            }
        }

        public void LogKSPContext() {
            LOGGER_CONTEXT.LogDebug("   ");
            LOGGER_CONTEXT.LogDebug("KSP Context : ");
            LOGGER_CONTEXT.LogDebug("- Current Scene : " + SceneManager.GetActiveScene().name);
            LOGGER_CONTEXT.LogDebug("- HighLogic :");
            LOGGER_CONTEXT.LogDebug("  - LoadedScene : " + HighLogic.LoadedScene.ToString());
            LOGGER_CONTEXT.LogDebug("  - LoadedSceneHasPlanetarium : " + HighLogic.LoadedSceneHasPlanetarium);
            LOGGER_CONTEXT.LogDebug("  - LoadedSceneIsEditor : " + HighLogic.LoadedSceneIsEditor);
            LOGGER_CONTEXT.LogDebug("  - LoadedSceneIsFlight : " + HighLogic.LoadedSceneIsFlight);
            LOGGER_CONTEXT.LogDebug("  - LoadedSceneIsGame : " + HighLogic.LoadedSceneIsGame);
            LOGGER_CONTEXT.LogDebug("  - LoadedSceneIsMissionBuilder : " + HighLogic.LoadedSceneIsMissionBuilder);
            
            LOGGER_CONTEXT.LogDebug("- MapView : " + MapView.MapIsEnabled);

            LOGGER_CONTEXT.LogDebug("- FlightUIMode present : " + (FlightUIModeController.Instance != null));
            if( FlightUIModeController.Instance != null ) {
                LOGGER_CONTEXT.LogDebug("  FlightUIMode : " + FlightUIModeController.Instance.Mode.ToString());
            }

            LOGGER_CONTEXT.LogDebug("- Active Vessel present : " + (FlightGlobals.ActiveVessel != null));
            if( FlightGlobals.ActiveVessel != null ) {
                LOGGER_CONTEXT.LogDebug("  Active Vessel : " + FlightGlobals.ActiveVessel.name);
                LOGGER_CONTEXT.LogDebug("  Active Vessel is EVA : " + FlightGlobals.ActiveVessel.isEVA);
            }

            LOGGER_CONTEXT.LogDebug("- EditorFacility : " + EditorDriver.editorFacility.ToString());
            
            LOGGER_CONTEXT.LogDebug("- CameraManager present : " + (CameraManager.Instance != null));
            if( CameraManager.Instance != null ) {
                LOGGER_CONTEXT.LogDebug("  CameraMode : " + CameraManager.Instance.currentCameraMode.ToString());
            }
        }

        // ====================================================================================

        // <summary>
        //  Update the action group to use, depending on the activated contexts
        // </summary>
        private void UpdateActionGroup() 
        {
            if( !this.steamControllerDaemon.ControllerConnected) {
                LOGGER.LogInfo("UpdateActionGroup: Controller not connected");
                return;
            }

            if( this.activecontexts.Count == 0 ) {
                this.TriggerActionGroupChange(DEFAULT_ACTION_GROUP);
            } else {
                ActionGroup last = this.activecontexts[this.activecontexts.Count - 1].CorrespondingActionGroup();
                this.TriggerActionGroupChange(last);
            }
        }
        
        // ====================================================================================
        
        // <summary>
        //  Trigger an action group change
        //  <param name="actionGroup">The action group to apply</param>
        // </summary>
        public void TriggerActionGroupChange(ActionGroup actionGroup) 
        {
            if( !this.steamControllerDaemon.ControllerConnected ) {
                LOGGER.LogInfo("TriggerActionGroupChange: Controller not connected");
                return;
            }
            
            this.CancelActionGroupChange();
            
            this.actionGroupToSet = actionGroup;
            this.delayedActionDaemon.TriggerDelayedAction(this._TriggerActionGroupChange, DELAY);
        }

        // <summary>
        //  Change the action group NOW
        //  <param name="actionGroup">The action group to apply</param>
        // </summary>
        public void ChangeActionGroupNow(ActionGroup actionGroup) 
        {
            if( !this.steamControllerDaemon.ControllerConnected ) {
                LOGGER.LogInfo("ChangeActionGroupNow: Controller not connected");
                return;
            }
            
            this.CancelActionGroupChange();
            
            this.actionGroupToSet = actionGroup;
            this._SetActionGroup(actionGroup);
        }

        private void _TriggerActionGroupChange() 
        {
            if( this.actionGroupToSet == ActionGroup.None ) {
                LOGGER.LogError("No action group to set");
                return;
            }
            this._SetActionGroup(this.actionGroupToSet);
            this.actionGroupToSet = ActionGroup.None;
        }

        // <summary>
        //  Cancel an action group change
        // </summary>
        private void CancelActionGroupChange() 
        {
            this.delayedActionDaemon.CancelDelayedAction(this._TriggerActionGroupChange);
            this.actionGroupToSet = ActionGroup.None;
        }

        private void _SetActionGroup(ActionGroup actionGroup) 
        {
            if( actionGroup == ActionGroup.None ) {
                LOGGER.LogError("Action group is None");
                return;
            }
            
            if( !this.steamControllerDaemon.ControllerConnected ) {
                LOGGER.LogError("Controller not connected");
                return;
            }

            if( this.prevActionGroup != ActionGroup.None )
            {
                if( actionGroup == this.prevActionGroup ) {
                    return;
                }
            }
            
            LOGGER.LogDebug("Setting action group : " + actionGroup.ToString());
            this.steamControllerDaemon.ChangeActionSet(actionGroup.ToString());
            
            this.screenMessage.message = "Controller: " + actionGroup.ToString() + ".";
            ScreenMessages.PostScreenMessage(this.screenMessage);
            
            this.prevActionGroup = actionGroup;
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
            this.prevActionGroup = ActionGroup.None;
            this.actionGroupToSet = ActionGroup.None;
            this.UpdateActionGroup();
        }

        // <summary>
        //  Controller disconnected
        // </summary>
        private void OnControllerDisconnected() 
        {
            LOGGER.LogInfo("Controller disconnected");
            this.CancelActionGroupChange();
            this.actionGroupToSet = ActionGroup.None;
            this.prevActionGroup = ActionGroup.None;
        }

    }
}
