using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine.SceneManagement;

namespace com.github.lhervier.ksp 
{
    // <summary>
    //  This class is a context daemon that detects when the game is in FreeIVA mode
    // </summary>
    public class FreeIVACtxDaemon : BaseContextDaemon
    {
        private static readonly SteamControllerLogger LOGGER = new SteamControllerLogger("FreeIVACtxDaemon");
        private static FreeIVACtxDaemon _instance;
        public static FreeIVACtxDaemon Instance {
            get {
                return _instance;
            }
        }

        private Type kerbalIvaAddonType;
        PropertyInfo instanceProperty;
        FieldInfo buckledProperty;
        private bool initialized = false;
        private bool ivaBeforePause = false;
        private bool inIva = false;
        
        public override ActionGroup CorrespondingActionGroup()
        {
            return ActionGroup.FreeIvaControls;
        }

        public void Awake()
        {
            _instance = this;
        }

        protected void Start() 
        {   
            LOGGER.LogInfo("Starting");
            kerbalIvaAddonType = Type.GetType("FreeIva.KerbalIvaAddon, FreeIva");
            if (kerbalIvaAddonType == null) {
                LOGGER.LogInfo("=> FreeIva mod not found");
                return;
            }
            
            instanceProperty = kerbalIvaAddonType.GetProperty("Instance");
            if (instanceProperty == null) {
                LOGGER.LogError("=> Instance property not found. FreeIVA mod has probably evolved...");
                return;
            }
            
            buckledProperty = kerbalIvaAddonType.GetField("buckled", BindingFlags.Public | BindingFlags.Instance);
            if (buckledProperty == null) {
                LOGGER.LogError("=> buckled field not found. FreeIVA mod has probably evolved...");
                return;
            }

            LOGGER.LogInfo("=> FreeIVA mod found");
            initialized = true;

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        protected void OnDestroy() 
        {
            LOGGER.LogInfo("OnDestroy");
            if( !initialized ) {
                return;
            }

            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            this.initialized = false;
            _instance = null;
        }

        bool InFreeIva() {
            if( !this.inIva ) {
                return false;
            }

            object instance = instanceProperty.GetValue(null);
            if( instance == null ) {
                return false;
            }
            return !(bool) buckledProperty.GetValue(instance);
        }

        protected void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            LOGGER.LogDebug("OnSceneLoaded : " + scene.name);
            if( scene.name.ToUpper() != "PFLIGHT4") return;

            GameEvents.onGamePause.Add(OnGamePause);
            GameEvents.onGameUnpause.Add(OnGameUnpause);

            GameEvents.OnFlightUIModeChanged.Add(OnFlightUIModeChanged);
            GameEvents.onVesselChange.Add(OnVesselChange);
        }

        protected void OnSceneUnloaded(Scene scene)
        {
            LOGGER.LogDebug("OnSceneUnloaded : " + scene.name);
            if( scene.name.ToUpper() != "PFLIGHT4") return;
            
            GameEvents.onGamePause.Remove(OnGamePause);
            GameEvents.onGameUnpause.Remove(OnGameUnpause);
            GameEvents.OnFlightUIModeChanged.Remove(OnFlightUIModeChanged);
            GameEvents.onVesselChange.Remove(OnVesselChange);
            this.inIva = false;
            
            this.FireContextEnterOrLeave(false);
        }

        private void OnGamePause()
        {
            LOGGER.LogDebug("=> OnGamePause");
            this.ivaBeforePause = this.InContext();
            this.FireContextEnterOrLeave(false);
        }

        private void OnGameUnpause()
        {
            LOGGER.LogDebug("=> OnGameUnpause");
            this.FireContextEnterOrLeave(this.ivaBeforePause);
        }

        private void OnFlightUIModeChanged(FlightUIMode mode)
        {
            LOGGER.LogDebug("=> OnFlightUIModeChanged : " + mode);
            this.inIva = InIVA();
        }

        private void OnVesselChange(Vessel vessel)
        {
            LOGGER.LogDebug("=> OnVesselChange : " + vessel.name);
            this.inIva = InIVA();
        }

        protected void FixedUpdate()
        {
            if( !initialized ) {
                return;
            }
            this.FireContextEnterOrLeave(InFreeIva());
        }
    }
}