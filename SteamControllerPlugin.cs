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

        // <summary>
        //  Trigger an action set change
        // </summary>
        public void TriggerActionSetChange() 
        {
            this.delayedActionDaemon.TriggerDelayedAction(this._TriggerActionSetChange, DELAY);
        }
        private void _TriggerActionSetChange() 
        {
            this._SetActionSet(this.ComputeActionSet());
        }

        // <summary>
        //  Cancel an action set change
        // </summary>
        private void CancelActionSetChange() 
        {
            this.delayedActionDaemon.CancelDelayedAction(this._TriggerActionSetChange);
        }

        // <summary>
        //  Change action set NOW
        // </summary>
        public void SetActionSet() 
        {
            this.CancelActionSetChange();
            this._SetActionSet(this.ComputeActionSet());
        }
        
        // <summary>
        //  Change action set NOW
        // </summary>
        // <param name="actionSetName">The name of the action set to set</param>
        public void SetActionSet(string actionSetName) 
        {
            this.CancelActionSetChange();
            this._SetActionSet(actionSetName);
        }
        private void _SetActionSet(string actionSetName) 
        {
            if( !this.connectionDaemon.ControllerConnected ) {
                return;
            }
            if( actionSetName == this.prevActionSet ) 
            {
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
        private string ComputeActionSet() 
        {
            foreach(IKspActionSet actionSet in this.actionSets) 
            {
                if( actionSet.Active() ) 
                {
                    return actionSet.ControlName();
                }
            }
            return this.defaultActionSet.ControlName();
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
            this.TriggerActionSetChange();
        }

        // <summary>
        //  Will be fired when pause main menu is displayed, but also when entering
        //  astronaut complex, R&D, Mission Control or administration building.
        // </summary>
        protected void OnGamePause() 
        {
            this.SetActionSet("MenuControls");
        }
        
        // <summary>
        //  Will be fired when game is unpaused, but also when leaving
        //  astronaut complex, R&D, Mission Control or administration building 
        // </summary>
        protected void OnGameUnpause() 
        {
            this.SetActionSet();
        }
        
        // <summary>
        //  User toggle the flightUI buttons (staging, docking, maps or maneuvre)
        // </summary>
        protected void OnFlightUIModeChanged(FlightUIMode mode) 
        {
            this.TriggerActionSetChange();
        }

        // <summary>
        //  Map mode entered (mainly in tracking station)
        // </summary>
        protected void OnMapEntered() 
        {
            this.SetActionSet("MapControls");       // FIXME
        }

        // <summary>
        //  Vessel changed
        // </summary>
        protected void OnVesselChange(Vessel ves) 
        {
            this.TriggerActionSetChange();
        }
    }
}
