using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Steamworks;

namespace com.github.lhervier.ksp 
{
    /// <summary>
    /// Daemon in charge of listening to controller connection/disconnection
    /// It also allow to change the current action set of the controller
    /// </summary>
    public class SteamInputDaemon : MonoBehaviour 
    {
        
        // ==========================================================================================
        //                          Static properties
        // ==========================================================================================

        /// <summary>
        /// Logger object
        /// </summary>
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("SteamInputDaemon");

        private static SteamInputDaemon _instance;
        public static SteamInputDaemon Instance {
            get {
                return _instance;
            }
        }

        // ==========================================================================================

        /// <summary>
        /// Called when a new controller is connected
        /// </summary>
        public readonly EventVoid OnControllerConnected = new EventVoid("SteamInputDaemon.OnControllerConnected");

        /// <summary>
        /// Called when a controller is disconnected
        /// </summary>
        public readonly EventVoid OnControllerDisconnected = new EventVoid("SteamInputDaemon.OnControllerDisconnected");

        /// <summary>
        /// Called when an error occurs and a new controller cannot be connected
        /// </summary>
        public readonly EventData<string> OnControllerConnectedWithError = new EventData<string>("SteamInputDaemon.OnControllerConnectedWithError");

        /// <summary>
        /// Is a controller connected ?
        /// </summary>
        public bool ControllerConnected { get; private set; }

        /// <summary>
        /// Is a controller connected with an error ?
        /// </summary>
        public bool ControllerConnectedWithErrors { get; private set; }

        /// <summary>
        /// The current action set
        /// </summary>
        public string CurrentActionSet { get; private set; }

        // ==============================================

        private string[] actionSets;

        // <summary>
        //  Handle to the first connected controller. No sense if ControllerConnected = false
        // </summary>
        private ControllerHandle_t controllerHandle;

        // <summary>
        //  The action sets handles defined in the steam controller configuration template
        // </summary>
        private readonly IDictionary<string, ControllerActionSetHandle_t> actionsSetsHandles = new Dictionary<string, ControllerActionSetHandle_t>();

        // <summary>
        //  Handles to the connected steam controllers.
        //  Don't use. This array is here to prevent from instanciating a new one every cycle.
        // </summary>
        private ControllerHandle_t[] _controllerHandles = new ControllerHandle_t[Constants.STEAM_CONTROLLER_MAX_COUNT];

        // =======================================================================

        /// <summary>
        /// Coroutine to check for a controller
        /// </summary>
        private IEnumerator checkForControllerCoroutine;

        // =======================================================================
        //              Unity Lifecycle
        // =======================================================================

        /// <summary>
        /// Component awaked
        /// </summary>
        public void Awake() 
        {
            DontDestroyOnLoad(this);
            _instance = this;
            LOGGER.LogInfo("Awaked");
        }

        /// <summary>
        /// Startup of the component
        /// </summary>
        public void Start() 
        {
            LOGGER.LogInfo("Starting");

            this.ControllerConnected = false;
            this.ControllerConnectedWithErrors = false;
            this.CurrentActionSet = null;

            LOGGER.LogInfo("Checking that Steam is initialized");
            if( !SteamManager.Initialized ) 
            {
                LOGGER.LogInfo("Steam not detected. Unable to start the daemon.");
                return;
            }

            // Load the action sets from the enumeration
            LOGGER.LogInfo("Loading action sets");
            this.actionSets = Enum.GetValues(typeof(ActionGroup))
                .Cast<ActionGroup>()
                .Where(actionGroup => actionGroup != ActionGroup.None)
                .Select(actionGroup => actionGroup.ToString())
                .ToArray();
            LOGGER.LogInfo("Action sets loaded : " + this.actionSets.Length);
            
            // Initialize the Steam Controller
            if( !Steamworks.SteamController.Init() )
            {
                LOGGER.LogError("Steam is not initialized. Unable to start the daemon.");
                return;
            }
            
            // Start the main loop
            this.checkForControllerCoroutine = this.CheckForController();
            this.StartCoroutine(this.checkForControllerCoroutine);
            
            LOGGER.LogInfo("Started");
        }

        /// <summary>
        /// Component destroyed
        /// </summary>
        public void OnDestroy() 
        {
            this.StopCoroutine(this.checkForControllerCoroutine);
            this.CurrentActionSet = null;
            this.ControllerConnected = false;
            this.ControllerConnectedWithErrors = false;
            _instance = null;
            LOGGER.LogInfo("Destroyed");
        }

        // ==============================================================================
        //              Detection of connection/disconnection of controllers
        // ==============================================================================
        
        /// <summary>
        /// Main loop to detect controller connection/disconnection
        /// </summary>
        private IEnumerator CheckForController() 
        {
            WaitForSeconds waitFor1Second = new WaitForSeconds(1);
            while( true ) 
            {
                Steamworks.SteamController.RunFrame();

                // Detect connection/disconnection
                LOGGER.LogTrace("Detecting controllers connection/disconnection :");
                int nbControllers = Steamworks.SteamController.GetConnectedControllers(this._controllerHandles);
                LOGGER.LogTrace("- nbControllers connected: " + nbControllers);
                bool newController;
                bool disconnectedController;
                if( nbControllers == 0 ) 
                {
                    LOGGER.LogTrace("- No controller connected");
                    if( this.ControllerConnected ) 
                    {
                        LOGGER.LogDebug("  A controller was previously connected");
                        newController = false;
                        disconnectedController = true;
                    }
                    else
                    {
                        LOGGER.LogTrace("  No controller previously connected");
                        newController = false;
                        disconnectedController = false;
                    }
                }
                else
                {
                    LOGGER.LogTrace("- A controller is connected");
                    if( this.ControllerConnected ) 
                    {
                        if( this.controllerHandle == this._controllerHandles[0] ) 
                        {
                            LOGGER.LogTrace("  The same controller is connected");
                            newController = false;
                            disconnectedController = false;
                        }
                        else
                        {
                            LOGGER.LogDebug("  A different controller is connected");
                            newController = true;
                            disconnectedController = true;
                        }
                    }
                    else
                    {
                        LOGGER.LogTrace("  No controller previously connected");
                        newController = true;
                        disconnectedController = false;
                    }
                }
                LOGGER.LogTrace("- newController: " + newController);
                LOGGER.LogTrace("- disconnectedController: " + disconnectedController); 

                // Disconnect the current controller
                if( disconnectedController ) 
                {
                    LOGGER.LogInfo("Controller disconnected");
                    this.UnloadActionSets();
                    this.ControllerConnected = false;
                    this.ControllerConnectedWithErrors = false;
                    this.CurrentActionSet = null;
                    this.OnControllerDisconnected.Fire();
                }

                // Connects a new controller
                if( newController ) 
                {
                    LOGGER.LogInfo("Controller connected");
                    this.controllerHandle = this._controllerHandles[0];
                    this.ControllerConnected = true;
                    this.ControllerConnectedWithErrors = !this.LoadActionSetsHandles();
                    if( this.ControllerConnectedWithErrors ) 
                    {
                        this.ControllerConnected = false;
                        this.CurrentActionSet = null;
                        this.OnControllerConnectedWithError.Fire("Unable to load action sets handles.");
                        yield break;
                    }
                    this.StartCoroutine(this.SayHello());
                    this.OnControllerConnected.Fire();
                }

                // Wait for 1 second
                yield return waitFor1Second;
            }
        }
        
        /// <summary>
        /// Load action sets handles.
        /// </summary>
        /// <returns>True if the action sets handles were loaded, false otherwise</returns>
        private bool LoadActionSetsHandles() 
        {
            LOGGER.LogInfo("Loading Action Set Handles");
            foreach(string actionSetName in this.actionSets) 
            {
                LOGGER.LogInfo("- Getting action set handle for " + actionSetName);
                // Action Sets list should depend on the used controller. But that's not what the API is waiting for...
                ControllerActionSetHandle_t actionSetHandle = Steamworks.SteamController.GetActionSetHandle(actionSetName);
                if( actionSetHandle.m_ControllerActionSetHandle == 0L ) 
                {
                    return false;
                }
                this.actionsSetsHandles[actionSetName] = actionSetHandle;
            }
            return true;
        }

        /// <summary>
        /// Unloads the action sets
        /// </summary>
        private void UnloadActionSets() 
        {
            this.actionsSetsHandles.Clear();
        }

        // <summary>
        //  Trigger a set of pulses on the current controller to say hello
        // </summary>
        private IEnumerator SayHello() 
        {
            if( !this.ControllerConnected ) 
            {
                LOGGER.LogError("SayHello: Controller not connected");
                yield break;
            }

            LOGGER.LogInfo("Hello new Controller !!");
            for( int i = 0; i < 4; i++ ) 
            {
                Steamworks.SteamController.TriggerHapticPulse(this.controllerHandle, Steamworks.ESteamControllerPad.k_ESteamControllerPad_Right, ushort.MaxValue);
                yield return new WaitForSeconds(0.1f);
                Steamworks.SteamController.TriggerHapticPulse(this.controllerHandle, Steamworks.ESteamControllerPad.k_ESteamControllerPad_Left, ushort.MaxValue);
                yield return new WaitForSeconds(0.1f);
            }
        }

        // =========================================================================================

        // <param name="actionSetName">The name of the action set to set</param>
        // <summary>
        //  Change the current action set
        // </summary>
        public void ChangeActionSet(string actionSetName) 
        {
            if( !this.ControllerConnected ) 
            {
                LOGGER.LogError("ChangeActionSet: Controller not connected");
                return;
            }
            
            LOGGER.LogDebug("ChangeActionSet: " + actionSetName);
            Steamworks.SteamController.ActivateActionSet(
                this.controllerHandle, 
                this.actionsSetsHandles[actionSetName]
            );
            this.CurrentActionSet = actionSetName;
        }
    }
}