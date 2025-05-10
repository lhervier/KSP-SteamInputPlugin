using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine.SceneManagement;
using SteamController;

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
            LOGGER.Log("Starting");
            kerbalIvaAddonType = Type.GetType("FreeIva.KerbalIvaAddon, FreeIva");
            if (kerbalIvaAddonType == null) {
                LOGGER.Log("=> FreeIva mod not found");
                return;
            }
            
            instanceProperty = kerbalIvaAddonType.GetProperty("Instance");
            if (instanceProperty == null) {
                LOGGER.Log("=> Instance property not found. FreeIVA mod has probably evolved...");
                return;
            }
            
            buckledProperty = kerbalIvaAddonType.GetField("buckled", BindingFlags.Public | BindingFlags.Instance);
            if (buckledProperty == null) {
                LOGGER.Log("=> buckled field not found. FreeIVA mod has probably evolved...");
                return;
            }

            LOGGER.Log("=> FreeIVA mod found");
            initialized = true;

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        protected void OnDestroy() 
        {
            LOGGER.Log("OnDestroy");
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
            // LOGGER.Log("OnSceneLoaded : " + scene.name);
            if( scene.name.ToUpper() != "PFLIGHT4") return;

            GameEvents.onGamePause.Add(OnGamePause);
            GameEvents.onGameUnpause.Add(OnGameUnpause);

            GameEvents.OnFlightUIModeChanged.Add(OnFlightUIModeChanged);
            GameEvents.onVesselChange.Add(OnVesselChange);
        }

        protected void OnSceneUnloaded(Scene scene)
        {
            // LOGGER.Log("OnSceneUnloaded : " + scene.name);
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
            // LOGGER.Log("=> OnGamePause");
            this.ivaBeforePause = this.InContext();
            this.FireContextEnterOrLeave(false);
        }

        private void OnGameUnpause()
        {
            // LOGGER.Log("=> OnGameUnpause");
            this.FireContextEnterOrLeave(this.ivaBeforePause);
        }

        private void OnFlightUIModeChanged(FlightUIMode mode)
        {
            // LOGGER.Log("=> OnFlightUIModeChanged : " + mode);
            this.inIva = InIVA();
        }

        private void OnVesselChange(Vessel vessel)
        {
            // LOGGER.Log("=> OnVesselChange : " + vessel.name);
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