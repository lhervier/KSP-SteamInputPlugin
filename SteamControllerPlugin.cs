using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine.SceneManagement;

namespace com.github.lhervier.ksp 
{
    [KSPAddon(KSPAddon.Startup.PSystemSpawn, true)]
    public class SteamControllerPlugin : MonoBehaviour 
    {
        
        // <summary>
        //  Logger
        // </summary>
        private static readonly SteamControllerLogger LOGGER = new SteamControllerLogger();
        private static readonly SteamControllerLogger LOGGER_CONTEXT = new SteamControllerLogger("Contexts");
        
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
        private SteamControllerDaemon connectionDaemon;
        
        // <summary>
        //  Delayed Action daemon
        // </summary>
        private DelayedActionDaemon delayedActionDaemon;

        // <summary>
        //  The action group to set when triggering a delayed action
        // </summary>
        private ActionGroup actionGroupToSet;

        // ===============================================================================
        //                      Unity initialization
        // ===============================================================================

        // <summary>
        //  Make our plugin survive between scene loading
        // </summary>
        protected void Awake() 
        {
            LOGGER.Log("Awaked");
            DontDestroyOnLoad(this);
        }

        // <summary>
        //  Start of the plugin
        // </summary>
        protected void Start() 
        {   
            LOGGER.Log("Starting");

            // Start the coroutine to handle the KSPSteamController
            StartCoroutine(HandleKSPSteamController());

            // Create the controller daemon
            this.connectionDaemon = gameObject.AddComponent<SteamControllerDaemon>();
            LOGGER.Log("Controller Daemon attached");
            this.connectionDaemon.OnControllerConnected.Add(this.OnControllerConnected);
            this.connectionDaemon.OnControllerDisconnected.Add(this.OnControllerDisconnected);
            LOGGER.Log("Controller Events attached");
            if( this.connectionDaemon.ControllerConnected ) 
            {
                LOGGER.Log("Controller already connected at startup");
                this.OnControllerConnected();
            }

            // Create the delayed action daemon
            this.delayedActionDaemon = gameObject.AddComponent<DelayedActionDaemon>();
            LOGGER.Log("Delayed Actions Daemon attached");
            this.actionGroupToSet = ActionGroup.None;
            this.prevActionGroup = ActionGroup.None;
            
            // Prepare screen message
            this.screenMessage = new ScreenMessage(
                string.Empty, 
                5f, 
                ScreenMessageStyle.UPPER_RIGHT
            );
            LOGGER.Log("Status message ready");

            // Get all the daemons and attach them to the plugin
            this.LoadContextDaemons();
            this.activecontexts.Clear();
            foreach(BaseContextDaemon daemon in this.contextDaemons) 
            {
                daemon.OnEnterContext().Add(this.OnEnterContext);
                daemon.OnExitContext().Add(this.OnExitContext);
            }
            LOGGER_CONTEXT.Log("Daemons attached : " + this.contextDaemons.Count);
            this.LogDaemons();
            
            LOGGER.Log("Started");
        }

        // <summary>
        //  Plugin destroyed
        // </summary>
        public void OnDestroy() 
        {
            this.connectionDaemon.OnControllerDisconnected.Remove(OnControllerDisconnected);
            this.connectionDaemon.OnControllerConnected.Remove(OnControllerConnected);
            Destroy(this.delayedActionDaemon);
            Destroy(this.connectionDaemon);
            
            foreach(BaseContextDaemon daemon in this.contextDaemons) 
            {
                daemon.OnEnterContext().Remove(this.OnEnterContext);
                daemon.OnExitContext().Remove(this.OnExitContext);
                Destroy((MonoBehaviour) daemon);
            }
            this.contextDaemons.Clear();
            this.activecontexts.Clear();
            LOGGER.Log("Destroyed");
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
            
            // Wait for steam initialization
            yield return new WaitUntil(() => {
                try {
                    Assembly kspSteamCtrlr = Assembly.Load("KSPSteamCtrlr");
                    if (kspSteamCtrlr == null) return false;
                    
                    Type controllerType = kspSteamCtrlr.GetType("KSPSteamController");
                    if (controllerType == null) return false;
                    
                    var controller = FindObjectOfType(controllerType) as MonoBehaviour;
                    return controller != null && controller.gameObject.activeInHierarchy;
                }
                catch {
                    return false;
                }
            });
            
            // Desactive le plugin SteamController par défaut
            try {
                Assembly kspSteamCtrlr = Assembly.Load("KSPSteamCtrlr");
                if (kspSteamCtrlr != null) {
                    Type controllerType = kspSteamCtrlr.GetType("KSPSteamController");
                    if (controllerType != null) {
                        var kspSteamController = FindObjectOfType(controllerType) as MonoBehaviour;
                        if (kspSteamController != null) {
                            LOGGER.Log("Desactivating Squad Steam Controller plugin");
                            // Stop any running coroutines first
                            kspSteamController.StopAllCoroutines();
                            // Then disable the component
                            kspSteamController.enabled = false;
                            // Finally deactivate the game object
                            kspSteamController.gameObject.SetActive(false);
                        }
                    }
                }
            }
            catch (Exception ex) {
                LOGGER.Log("Error disabling Squad Steam Controller: " + ex.Message);
            }
        }

        // ====================================================================================

        // <summary>
        //  When a context is activated
        // </summary>
        public void OnEnterContext(BaseContextDaemon daemon)
        {
            LOGGER_CONTEXT.Log("OnEnterContext : " + daemon.GetType().Name);
            this.activecontexts.Add(daemon);
            // this.LogKSPContext();
            this.LogDaemons();
            this.UpdateActionGroup();
        }

        // <summary>
        //  When a context is deactivated
        // </summary>
        public void OnExitContext(BaseContextDaemon daemon)
        {
            LOGGER_CONTEXT.Log("OnExitContext : " + daemon.GetType().Name);
            this.activecontexts.Remove(daemon);
            // this.LogKSPContext();
            this.LogDaemons();
            this.UpdateActionGroup();
        }

        public void LogDaemons()
        {
            if( this.activecontexts.Count == 0 ) {
                LOGGER_CONTEXT.Log("No active daemons contexts");
            } else {
                LOGGER_CONTEXT.Log("Active daemons contexts: " + this.activecontexts.Count);
                foreach( BaseContextDaemon daemon in this.activecontexts ) {
                    LOGGER_CONTEXT.Log("- " + daemon.GetType().Name);
                }
            }
        }

        public void LogKSPContext() {
            LOGGER_CONTEXT.Log("   ");
            LOGGER_CONTEXT.Log("KSP Context : ");
            LOGGER_CONTEXT.Log("- Current Scene : " + SceneManager.GetActiveScene().name);
            LOGGER_CONTEXT.Log("- HighLogic :");
            LOGGER_CONTEXT.Log("  - LoadedScene : " + HighLogic.LoadedScene.ToString());
            LOGGER_CONTEXT.Log("  - LoadedSceneHasPlanetarium : " + HighLogic.LoadedSceneHasPlanetarium);
            LOGGER_CONTEXT.Log("  - LoadedSceneIsEditor : " + HighLogic.LoadedSceneIsEditor);
            LOGGER_CONTEXT.Log("  - LoadedSceneIsFlight : " + HighLogic.LoadedSceneIsFlight);
            LOGGER_CONTEXT.Log("  - LoadedSceneIsGame : " + HighLogic.LoadedSceneIsGame);
            LOGGER_CONTEXT.Log("  - LoadedSceneIsMissionBuilder : " + HighLogic.LoadedSceneIsMissionBuilder);
            
            LOGGER_CONTEXT.Log("- MapView : " + MapView.MapIsEnabled);

            LOGGER_CONTEXT.Log("- FlightUIMode present : " + (FlightUIModeController.Instance != null));
            if( FlightUIModeController.Instance != null ) {
                LOGGER_CONTEXT.Log("  FlightUIMode : " + FlightUIModeController.Instance.Mode.ToString());
            }

            LOGGER_CONTEXT.Log("- Active Vessel present : " + (FlightGlobals.ActiveVessel != null));
            if( FlightGlobals.ActiveVessel != null ) {
                LOGGER_CONTEXT.Log("  Active Vessel : " + FlightGlobals.ActiveVessel.name);
                LOGGER_CONTEXT.Log("  Active Vessel is EVA : " + FlightGlobals.ActiveVessel.isEVA);
            }

            LOGGER_CONTEXT.Log("- EditorFacility : " + EditorDriver.editorFacility.ToString());
            
            LOGGER_CONTEXT.Log("- CameraManager present : " + (CameraManager.Instance != null));
            if( CameraManager.Instance != null ) {
                LOGGER_CONTEXT.Log("  CameraMode : " + CameraManager.Instance.currentCameraMode.ToString());
            }
        }

        // ====================================================================================

        // <summary>
        //  Update the action group to use, depending on the activated contexts
        // </summary>
        private void UpdateActionGroup() 
        {
            if( !this.connectionDaemon.ControllerConnected) {
                LOGGER.Log("UpdateActionGroup: Controller not connected");
                return;
            }

            if( this.activecontexts.Count == 0 ) {
                this.TriggerActionGroupChange(DEFAULT_ACTION_GROUP);
            } else if( this.activecontexts.Count > 1 ) {
                ActionGroup last = this.activecontexts[this.activecontexts.Count - 1].CorrespondingActionGroup();
                this.TriggerActionGroupChange(last);
            } else {
                ActionGroup unique = this.activecontexts[0].CorrespondingActionGroup();
                this.ChangeActionGroupNow(unique);
            }
        }
        
        // ====================================================================================
        
        // <summary>
        //  Trigger an action group change
        //  <param name="actionGroup">The action group to apply</param>
        // </summary>
        public void TriggerActionGroupChange(ActionGroup actionGroup) 
        {
            if( !this.connectionDaemon.ControllerConnected ) {
                LOGGER.Log("TriggerActionGroupChange: Controller not connected");
                return;
            }
            
            if( !this.connectionDaemon.ControllerConnected ) {
                LOGGER.Log("TriggerActionGroupChange: Controller not connected");
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
            if( !this.connectionDaemon.ControllerConnected ) {
                LOGGER.Log("ChangeActionGroupNow: Controller not connected");
                return;
            }
            
            this.CancelActionGroupChange();
            
            this.actionGroupToSet = actionGroup;
            this._SetActionGroup(actionGroup);
        }

        private void _TriggerActionGroupChange() 
        {
            if( this.actionGroupToSet == ActionGroup.None ) {
                LOGGER.Log("ERROR : No action group to set");
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
                LOGGER.Log("ERROR : Action group is None");
                return;
            }
            
            if( !this.connectionDaemon.ControllerConnected ) {
                LOGGER.Log("ERROR : Controller not connected");
                return;
            }

            if( this.prevActionGroup != ActionGroup.None )
            {
                if( actionGroup == this.prevActionGroup ) {
                    return;
                }
            }
            
            LOGGER.Log("Setting action group : " + actionGroup.ToString());
            this.connectionDaemon.ChangeActionSet(actionGroup.ToString());
            
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
            LOGGER.Log("New Controller connected");
            this.UpdateActionGroup();
        }

        // <summary>
        //  Controller disconnected
        // </summary>
        private void OnControllerDisconnected() 
        {
            LOGGER.Log("Controller disconnected");
            this.CancelActionGroupChange();
            this.actionGroupToSet = ActionGroup.None;
        }

    }
}
