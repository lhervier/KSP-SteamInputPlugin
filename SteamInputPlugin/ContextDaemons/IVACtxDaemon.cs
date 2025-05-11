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
    //  This class is a context daemon that detects when the game is in IVA mode
    // </summary>
    public class IVACtxDaemon : BaseContextDaemon
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("IVACtxDaemon");
        private bool ivaBeforePause = false;
        private bool inFreeIva = false;
        
        public override ActionGroup CorrespondingActionGroup()
        {
            return ActionGroup.IvaControls;
        }

        public void Start()
        {
            LOGGER.LogInfo("Start");

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        public void OnDestroy()
        {
            LOGGER.LogInfo("OnDestroy");

            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        protected void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            LOGGER.LogDebug("OnSceneLoaded : " + scene.name);
            if( scene.name.ToUpper() != "PFLIGHT4") return;
            
            this.inFreeIva = false;
            this.FireContextEnterOrLeave(false);

            GameEvents.OnMapEntered.Add(OnMapEntered);
            GameEvents.OnMapExited.Add(OnMapExited);
            
            this.OnMapExited();
        }

        protected void OnSceneUnloaded(Scene scene)
        {
            LOGGER.LogDebug("OnSceneUnloaded : " + scene.name);
            if( scene.name.ToUpper() != "PFLIGHT4" ) {
                return;
            }
            GameEvents.onGamePause.Remove(OnGamePause);
            GameEvents.onGameUnpause.Remove(OnGameUnpause);
            GameEvents.OnFlightUIModeChanged.Remove(OnFlightUIModeChanged);
            GameEvents.OnMapEntered.Remove(OnMapEntered);
            GameEvents.OnMapExited.Remove(OnMapExited);
            GameEvents.onVesselChange.Remove(OnVesselChange);

            this.inFreeIva = false;
            this.FireContextEnterOrLeave(false);
        }

        // ============================================================

        private void OnMapEntered()
        {
            LOGGER.LogTrace("=> OnMapEntered");
            GameEvents.onGamePause.Remove(OnGamePause);
            GameEvents.onGameUnpause.Remove(OnGameUnpause);
            GameEvents.OnFlightUIModeChanged.Remove(OnFlightUIModeChanged);
            GameEvents.onVesselChange.Remove(OnVesselChange);

            FreeIVACtxDaemon.Instance.OnEnterContext().Remove(OnEnterFreeIvaContext);
            FreeIVACtxDaemon.Instance.OnExitContext().Remove(OnExitFreeIvaContext);
            
            this.FireContextEnterOrLeave(false);
        }

        private void OnMapExited()
        {
            LOGGER.LogTrace("=> OnMapExited");
            GameEvents.onGamePause.Add(OnGamePause);
            GameEvents.onGameUnpause.Add(OnGameUnpause);
            GameEvents.OnFlightUIModeChanged.Add(OnFlightUIModeChanged);
            GameEvents.onVesselChange.Add(OnVesselChange);

            FreeIVACtxDaemon.Instance.OnEnterContext().Add(OnEnterFreeIvaContext);
            FreeIVACtxDaemon.Instance.OnExitContext().Add(OnExitFreeIvaContext);

            this.FireContextEnterOrLeave(
                this.InIVA()
            );
        }

        private void OnGamePause()
        {
            LOGGER.LogTrace("=> OnGamePause");
            this.ivaBeforePause = this.InContext();
            this.FireContextEnterOrLeave(false);
        }

        private void OnGameUnpause()
        {
            LOGGER.LogTrace("=> OnGameUnpause");
            this.FireContextEnterOrLeave(this.ivaBeforePause);
        }

        private void OnEnterFreeIvaContext(BaseContextDaemon sender)
        {
            LOGGER.LogTrace("=> OnEnterFreeIvaContext");
            this.inFreeIva = true;
            this.FireContextEnterOrLeave(false);
        }

        private void OnExitFreeIvaContext(BaseContextDaemon sender)
        {
            LOGGER.LogTrace("=> OnExitFreeIvaContext");
            this.inFreeIva = false;
            this.FireContextEnterOrLeave(
                this.InIVA()
            );
        }

        private void OnFlightUIModeChanged(FlightUIMode mode)
        {
            LOGGER.LogTrace("=> OnFlightUIModeChanged : " + mode.ToString());
            if( this.inFreeIva ) return;
            this.FireContextEnterOrLeave(
                this.InIVA()
            );
        }

        private void OnVesselChange(Vessel vessel)
        {
            LOGGER.LogTrace("=> OnVesselChange : " + vessel.name);
            if( this.inFreeIva ) return;
            this.FireContextEnterOrLeave(
                this.InIVA()
            );
        }
    }
}
