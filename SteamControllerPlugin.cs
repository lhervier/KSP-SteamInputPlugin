using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine.SceneManagement;

namespace com.github.lhervier.ksp 
{
    public enum SpacePortFacility {
        None,
        Administration,
        AstronautComplex,
        MissionControl,
        RnDComplex
    }

    public class ActionGroup {
        public string Name { get; set; }
        public RefreshType RefreshType { get; set; }
    }
    
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

        // <summary>
        //  Pour savoir si on est dans un building du space center
        // </summary>
        public static SpacePortFacility SpaceCenterBuilding = SpacePortFacility.None;

        // <summary>
        //  Pour savoir si on est en mode Construction en EVA
        // </summary>
        public static bool EVAConstructionMode = false;

        // <summary>
        //  Le nom de la scène actuellement chargée
        // </summary>
        public static string sceneName;

        // <summary>
        //  Pour savoir si on est en mode Editor
        // </summary>
        public static bool EditorMode = false;

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
            LOGGER.Log("- Current Scene : " + SceneManager.GetActiveScene().name);
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
            
            LOGGER.Log("- SpaceCenterBuilding : " + SpaceCenterBuilding.ToString());
            LOGGER.Log("- EVAConstructionMode : " + EVAConstructionMode);

            LOGGER.Log("Cancelling existing action set change (if any)");
            this.CancelActionSetChange();
            
            ActionGroup actionGroup = this.ComputeActionSet();
            if( actionGroup.RefreshType == RefreshType.Immediate ) {
                this._SetActionSet(actionGroup.Name);
            } else {
                this.actionSetToSet = actionGroup.Name;
                this.delayedActionDaemon.TriggerDelayedAction(this._TriggerActionSetChange, DELAY);
            }
        }
        private void _TriggerActionSetChange() 
        {
            LOGGER.Log("Triggering delayed action set change");
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
        private ActionGroup ComputeActionSet() 
        {
            LOGGER.Log("Computing action set");
            foreach(IKspActionSet actionSet in this.actionSets) 
            {
                LOGGER.Log("- " + actionSet.GetType().Name);
                RefreshType refreshType = actionSet.Active();
                if( refreshType == RefreshType.Nope ) 
                {
                    LOGGER.Log("  Nope...");
                    continue;
                }
                LOGGER.Log("  Found : " + refreshType.ToString() + "/" + actionSet.ControlName());
                return new ActionGroup { Name = actionSet.ControlName(), RefreshType = refreshType };
            }
            LOGGER.Log("No action set found. Using default");
            return new ActionGroup { Name = this.defaultActionSet.ControlName(), RefreshType = RefreshType.Delayed };
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

            GameEvents.onGUIAdministrationFacilitySpawn.Add(OnGUIAdministrationFacilitySpawn);
            GameEvents.onGUIAdministrationFacilityDespawn.Add(OnGUIAdministrationFacilityDespawn);
            GameEvents.onGUIAstronautComplexSpawn.Add(OnGUIAstronautComplexSpawn);
            GameEvents.onGUIAstronautComplexDespawn.Add(OnGUIAstronautComplexDespawn);
            GameEvents.onGUIMissionControlSpawn.Add(OnGUIMissionControlSpawn);
            GameEvents.onGUIMissionControlDespawn.Add(OnGUIMissionControlDespawn);
            GameEvents.onGUIRnDComplexSpawn.Add(OnGUIRnDComplexSpawn);
            GameEvents.onGUIRnDComplexDespawn.Add(OnGUIRnDComplexDespawn);
            
            GameEvents.OnEVAConstructionMode.Add(OnEVAConstructionModeChanged);

            SceneManager.sceneLoaded += OnSceneLoaded;

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

            GameEvents.onGUIAdministrationFacilitySpawn.Remove(OnGUIAdministrationFacilitySpawn);
            GameEvents.onGUIAdministrationFacilityDespawn.Remove(OnGUIAdministrationFacilityDespawn);
            GameEvents.onGUIAstronautComplexSpawn.Remove(OnGUIAstronautComplexSpawn);
            GameEvents.onGUIAstronautComplexDespawn.Remove(OnGUIAstronautComplexDespawn);
            GameEvents.onGUIMissionControlSpawn.Remove(OnGUIMissionControlSpawn);
            GameEvents.onGUIMissionControlDespawn.Remove(OnGUIMissionControlDespawn);
            GameEvents.onGUIRnDComplexSpawn.Remove(OnGUIRnDComplexSpawn);
            GameEvents.onGUIRnDComplexDespawn.Remove(OnGUIRnDComplexDespawn);
            
            GameEvents.OnEVAConstructionMode.Remove(OnEVAConstructionModeChanged);

            SceneManager.sceneLoaded -= OnSceneLoaded;

            LOGGER.Log("KSP hooks removed");
        }

        // ========================================================================================
        //                                      KSP Events
        // ========================================================================================

        protected void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            LOGGER.Log(" ");
            LOGGER.Log("=> OnSceneLoaded : " + scene.name);
            LOGGER.Log(" ");
            sceneName = scene.name;
            this.TriggerActionSetChange();
        }

        // <summary>
        //  A new scene has been loaded
        // </summary>
        protected void OnLevelWasLoadedGUIReady(GameScenes scn) 
        {
            LOGGER.Log(" ");
            LOGGER.Log("=> OnLevelWasLoadedGUIReady : " + scn.ToString());
            LOGGER.Log(" ");
            this.TriggerActionSetChange();
        }

        // <summary>
        //  Will be fired when pause main menu is displayed, but also when entering
        //  astronaut complex, R&D, Mission Control or administration building.
        // </summary>
        protected void OnGamePause() 
        {
            LOGGER.Log(" ");
            LOGGER.Log("=> OnGamePause");
            LOGGER.Log(" ");
            GamePaused = true;
            this.TriggerActionSetChange();
        }
        
        // <summary>
        //  Will be fired when game is unpaused, but also when leaving
        //  astronaut complex, R&D, Mission Control or administration building 
        // </summary>
        protected void OnGameUnpause() 
        {
            LOGGER.Log(" ");
            LOGGER.Log("=> OnGameUnpause");
            LOGGER.Log(" ");
            GamePaused = false;
            this.TriggerActionSetChange();
        }
        
        // <summary>
        //  User toggle the flightUI buttons (staging, docking, maps or maneuvre)
        // </summary>
        protected void OnFlightUIModeChanged(FlightUIMode mode) 
        {
            LOGGER.Log(" ");
            LOGGER.Log("=> OnFlightUIModeChanged : " + mode.ToString());
            LOGGER.Log(" ");
            this.TriggerActionSetChange();
        }

        // <summary>
        //  Map mode entered (mainly in tracking station)
        // </summary>
        protected void OnMapEntered() 
        {
            LOGGER.Log(" ");
            LOGGER.Log("=> OnMapEntered");
            LOGGER.Log(" ");
            this.TriggerActionSetChange();
        }

        // <summary>
        //  Map mode exited (mainly in tracking station)
        // </summary>
        protected void OnMapExited() 
        {
            LOGGER.Log(" ");
            LOGGER.Log("=> OnMapExited");
            LOGGER.Log(" ");
            this.TriggerActionSetChange();
        }

        // <summary>
        //  Vessel changed
        // </summary>
        protected void OnVesselChange(Vessel ves) 
        {
            LOGGER.Log(" ");
            LOGGER.Log("=> OnVesselChange");
            LOGGER.Log(" ");
            this.TriggerActionSetChange();
        }

        protected void OnGUIAdministrationFacilitySpawn()
        {
            LOGGER.Log(" ");
            LOGGER.Log("=> onGUIAdministrationFacilitySpawn");
            LOGGER.Log(" ");
            SpaceCenterBuilding = SpacePortFacility.Administration;
            this.TriggerActionSetChange();
        }

        protected void OnGUIAdministrationFacilityDespawn()
        {
            LOGGER.Log(" ");
            LOGGER.Log("=> onGUIAdministrationFacilityDespawn");
            LOGGER.Log(" ");
            SpaceCenterBuilding = SpacePortFacility.None;
            this.TriggerActionSetChange();
        }

        protected void OnGUIAstronautComplexSpawn()
        {
            LOGGER.Log(" ");
            LOGGER.Log("=> OnGUIAstronautComplexSpawn");
            LOGGER.Log(" ");
            SpaceCenterBuilding = SpacePortFacility.AstronautComplex;
            this.TriggerActionSetChange();
        }

        protected void OnGUIAstronautComplexDespawn()
        {
            LOGGER.Log(" ");
            LOGGER.Log("=> OnGUIAstronautComplexDespawn");
            LOGGER.Log(" ");
            SpaceCenterBuilding = SpacePortFacility.None;
            this.TriggerActionSetChange();
        }

        protected void OnGUIMissionControlSpawn()
        {
            LOGGER.Log(" ");
            LOGGER.Log("=> OnGUIMissionControlSpawn");
            LOGGER.Log(" ");
            SpaceCenterBuilding = SpacePortFacility.MissionControl;
            this.TriggerActionSetChange();
        }

        protected void OnGUIMissionControlDespawn()
        {
            LOGGER.Log(" ");
            LOGGER.Log("=> OnGUIMissionControlDespawn");
            LOGGER.Log(" ");
            SpaceCenterBuilding = SpacePortFacility.None;
            this.TriggerActionSetChange();
        }

        protected void OnGUIRnDComplexSpawn()
        {
            LOGGER.Log(" ");
            LOGGER.Log("=> OnGUIRnDComplexSpawn");
            LOGGER.Log(" ");    
            SpaceCenterBuilding = SpacePortFacility.RnDComplex;
            this.TriggerActionSetChange();
        }

        protected void OnGUIRnDComplexDespawn()
        {
            LOGGER.Log(" ");
            LOGGER.Log("=> OnGUIRnDComplexDespawn");
            LOGGER.Log(" ");
            SpaceCenterBuilding = SpacePortFacility.None;
            this.TriggerActionSetChange();
        }
        
        protected void OnEVAConstructionModeChanged(bool mode)
        {
            LOGGER.Log(" ");
            LOGGER.Log("=> OnEVAConstructionModeChanged : " + mode);
            LOGGER.Log(" ");
            EVAConstructionMode = mode;
            this.TriggerActionSetChange();
        }
    }
}
