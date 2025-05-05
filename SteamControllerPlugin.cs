using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace com.github.lhervier.ksp 
{
    
    [KSPAddon(KSPAddon.Startup.PSystemSpawn, true)]
    public class SteamControllerPlugin : MonoBehaviour 
    {
        
        // <summary>
        //  Logger
        // </summary>
        private static readonly SteamControllerLogger LOGGER = new SteamControllerLogger();
        
        // <summary>
        //  Delay before applying an action set (in frames)
        // </summary>
        private static readonly int DELAY = 10;
        
        // <summary>
        //  Pour savoir si le jeu est en pause...
        // </summary>
        public static bool GamePaused = false;

        // ==================================================================================

        // <summary>
        //  The action sets
        // </summary>
        private List<IKspActionSet> actionSets;

        // <summary>
        //  The default action set
        // </summary>
        private IKspActionSet defaultActionSet;

        // <summary>
        //  Message indicating when on Steam Controller action set changes
        // </summary>
        private ScreenMessage screenMessage;

        // <summary>
        //  Previous action set (so we don't display the message when the value has not changed)
        // </summary>
        private string prevActionSet;

        // <summary>
        //  Connection Daemon to the steam controller
        // </summary>
        private SteamControllerDaemon connectionDaemon;
        
        // <summary>
        //  Delayed Action daemon
        // </summary>
        private DelayedActionDaemon delayedActionDaemon;

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

            // Get all the action sets
            LOGGER.Log("Loading action sets");
            this.LoadActionSets();
            LOGGER.Log("Action sets loaded : " + this.actionSets.Count);
            foreach(IKspActionSet actionSet in this.actionSets) 
            {
                LOGGER.Log("- " + actionSet.ControlName());
            }
            LOGGER.Log("Default action set : " + this.defaultActionSet.ControlName());
            
            // Create the controller daemon
            this.connectionDaemon = gameObject.AddComponent<SteamControllerDaemon>();
            LOGGER.Log("Controller Daemon attached");

            // Create the delayed action daemon
            this.delayedActionDaemon = gameObject.AddComponent<DelayedActionDaemon>();
            LOGGER.Log("Delayed Actions Daemon attached");
            
            // Prepare screen message
            this.screenMessage = new ScreenMessage(
                string.Empty, 
                5f, 
                ScreenMessageStyle.UPPER_RIGHT
            );
            LOGGER.Log("Status message ready");

            // Attach to connection Daemon
            this.connectionDaemon.OnControllerConnected.Add(this.OnControllerConnected);
            this.connectionDaemon.OnControllerDisconnected.Add(this.OnControllerDisconnected);
            LOGGER.Log("Controller Events attached");

            // When a controller is already connected
            if( this.connectionDaemon.ControllerConnected ) 
            {
                this.OnControllerConnected();
            }
            LOGGER.Log("Started");
        }

        private void LoadActionSets()
        {
            // Get all types that implement IKspActionSet
            var actionSetTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IKspActionSet).IsAssignableFrom(t));

            // Create a list to store the action sets
            this.actionSets = new List<IKspActionSet>();

            // Add each action set component to the GameObject
            foreach (var type in actionSetTypes)
            {
                IKspActionSet component = gameObject.AddComponent(type) as IKspActionSet;
                this.actionSets.Add(component);
                if( component.Default() ) 
                {
                    this.defaultActionSet = component;
                }
            }
            if( this.defaultActionSet == null ) 
            {
                throw new Exception("No default action set found");
            }
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
            LOGGER.Log("Destroyed");
        }

        // ====================================================================================

        private string actionSetToSet;

        // <summary>
        //  Trigger an action set change
        // </summary>
        public void TriggerActionSetChange() 
        {
            LOGGER.Log("   ");
            LOGGER.Log("--------------------------------");
            LOGGER.Log("TriggerActionSetChange");
            LOGGER.Log("--------------------------------");
            LOGGER.Log("- HighLogic :");
            LOGGER.Log("  - LoadedScene : " + HighLogic.LoadedScene.ToString());
            LOGGER.Log("  - LoadedSceneHasPlanetarium : " + HighLogic.LoadedSceneHasPlanetarium);
            LOGGER.Log("  - LoadedSceneIsEditor : " + HighLogic.LoadedSceneIsEditor);
            LOGGER.Log("  - LoadedSceneIsFlight : " + HighLogic.LoadedSceneIsFlight);
            LOGGER.Log("  - LoadedSceneIsGame : " + HighLogic.LoadedSceneIsGame);
            LOGGER.Log("  - LoadedSceneIsMissionBuilder : " + HighLogic.LoadedSceneIsMissionBuilder);
            
            LOGGER.Log("- GamePaused : " + GamePaused);
            LOGGER.Log("- MapView : " + MapView.MapIsEnabled);

            LOGGER.Log("- FlightUIMode present : " + (FlightUIModeController.Instance != null));
            if( FlightUIModeController.Instance != null ) {
                LOGGER.Log("  FlightUIMode : " + FlightUIModeController.Instance.Mode.ToString());
            }

            LOGGER.Log("- Active Vessel present : " + (FlightGlobals.ActiveVessel != null));
            if( FlightGlobals.ActiveVessel != null ) {
                LOGGER.Log("  Active Vessel : " + FlightGlobals.ActiveVessel.name);
                LOGGER.Log("  Active Vessel is EVA : " + FlightGlobals.ActiveVessel.isEVA);
            }

            LOGGER.Log("- EditorFacility : " + EditorDriver.editorFacility.ToString());
            
            this.CancelActionSetChange();
            
            IKspActionSet actionSet = this.ComputeActionSet();
            if( actionSet.Active() == RefreshType.Immediate ) {
                this._SetActionSet(actionSet.ControlName());
            } else {
                this.actionSetToSet = actionSet.ControlName();
                this.delayedActionDaemon.TriggerDelayedAction(this._TriggerActionSetChange, DELAY);
            }
        }
        private void _TriggerActionSetChange() 
        {
            if( this.actionSetToSet != null ) {
                this._SetActionSet(this.actionSetToSet);
                this.actionSetToSet = null;
            } else {
                LOGGER.Log("ERROR : No action set to set");
            }
        }

        // <summary>
        //  Cancel an action set change
        // </summary>
        private void CancelActionSetChange() 
        {
            this.delayedActionDaemon.CancelDelayedAction(this._TriggerActionSetChange);
            this.actionSetToSet = null;
        }

        private void _SetActionSet(string actionSetName) 
        {
            LOGGER.Log("Setting action set : " + actionSetName);
            if( !this.connectionDaemon.ControllerConnected ) {
                LOGGER.Log("  Controller not connected");
                return;
            }
            if( actionSetName == this.prevActionSet ) 
            {
                LOGGER.Log("  Action set already set");
                return;
            }

            this.connectionDaemon.setActionSet(actionSetName);
            
            this.screenMessage.message = "Controller: " + actionSetName + ".";
            ScreenMessages.PostScreenMessage(this.screenMessage);
            this.prevActionSet = actionSetName;
        }

        // <summary>
        //  Compute the action set to use, depending on the KSP context
        // </summary>
        private IKspActionSet ComputeActionSet() 
        {
            LOGGER.Log("Computing action set");
            foreach(IKspActionSet actionSet in this.actionSets) 
            {
                LOGGER.Log("- " + actionSet.ControlName());
                RefreshType refreshType = actionSet.Active();
                if( refreshType == RefreshType.Nope ) 
                {
                    LOGGER.Log("  Nope...");
                    continue;
                }
                LOGGER.Log("  Found : " + refreshType.ToString());
                return actionSet;
            }
            LOGGER.Log("No action set found. Using default");
            return this.defaultActionSet;
        }
        
        // ==============================================================================
        //              Connection/disconnection events of controller
        // ==============================================================================
        
        // <summary>
        //  New controller connected
        // </summary>
        private void OnControllerConnected() 
        {
            // Hooks to KSP
            GameEvents.onLevelWasLoadedGUIReady.Add(OnLevelWasLoadedGUIReady);
            GameEvents.onGamePause.Add(OnGamePause);
            GameEvents.onGameUnpause.Add(OnGameUnpause);
            GameEvents.OnFlightUIModeChanged.Add(OnFlightUIModeChanged);
            GameEvents.OnMapEntered.Add(OnMapEntered);
            GameEvents.OnMapExited.Add(OnMapExited);
            GameEvents.onVesselChange.Add(OnVesselChange);
            LOGGER.Log("KSP hooks created");

            // Trigger an action set change to load the right action set
            this.TriggerActionSetChange();
        }

        // <summary>
        //  Controller disconnected
        // </summary>
        private void OnControllerDisconnected() 
        {
            // Canceling eventual action set change
            this.CancelActionSetChange();

            // Unhooks to KSP
            GameEvents.onLevelWasLoadedGUIReady.Remove(OnLevelWasLoadedGUIReady);
            GameEvents.onGamePause.Remove(OnGamePause);
            GameEvents.onGameUnpause.Remove(OnGameUnpause);
            GameEvents.OnFlightUIModeChanged.Remove(OnFlightUIModeChanged);
            GameEvents.OnMapEntered.Remove(OnMapEntered);
            GameEvents.OnMapExited.Remove(OnMapExited);
            GameEvents.onVesselChange.Remove(OnVesselChange);
            LOGGER.Log("KSP hooks removed");
        }

        // ========================================================================================
        //                                      KSP Events
        // ========================================================================================

        // <summary>
        //  A new scene has been loaded
        // </summary>
        protected void OnLevelWasLoadedGUIReady(GameScenes scn) 
        {
            LOGGER.Log("OnLevelWasLoadedGUIReady : " + scn.ToString());
            this.TriggerActionSetChange();
        }

        // <summary>
        //  Will be fired when pause main menu is displayed, but also when entering
        //  astronaut complex, R&D, Mission Control or administration building.
        // </summary>
        protected void OnGamePause() 
        {
            LOGGER.Log("OnGamePause");
            GamePaused = true;
            this.TriggerActionSetChange();
        }
        
        // <summary>
        //  Will be fired when game is unpaused, but also when leaving
        //  astronaut complex, R&D, Mission Control or administration building 
        // </summary>
        protected void OnGameUnpause() 
        {
            LOGGER.Log("OnGameUnpause");
            GamePaused = false;
            this.TriggerActionSetChange();
        }
        
        // <summary>
        //  User toggle the flightUI buttons (staging, docking, maps or maneuvre)
        // </summary>
        protected void OnFlightUIModeChanged(FlightUIMode mode) 
        {
            LOGGER.Log("OnFlightUIModeChanged : " + mode.ToString());
            this.TriggerActionSetChange();
        }

        // <summary>
        //  Map mode entered (mainly in tracking station)
        // </summary>
        protected void OnMapEntered() 
        {
            LOGGER.Log("OnMapEntered");
            this.TriggerActionSetChange();
        }

        // <summary>
        //  Map mode exited (mainly in tracking station)
        // </summary>
        protected void OnMapExited() 
        {
            LOGGER.Log("OnMapExited");
            this.TriggerActionSetChange();
        }

        // <summary>
        //  Vessel changed
        // </summary>
        protected void OnVesselChange(Vessel ves) 
        {
            LOGGER.Log("OnVesselChange");
            this.TriggerActionSetChange();
        }
    }
}
