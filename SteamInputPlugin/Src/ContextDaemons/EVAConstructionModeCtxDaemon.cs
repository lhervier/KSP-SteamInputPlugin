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
    //  This class is a context daemon that detects when the game is in EVA construction mode
    // </summary>
    public class EVAConstructionModeCtxDaemon : BaseContextDaemon
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("EVAConstructionModeCtxDaemon");

        public override ActionGroup CorrespondingActionGroup()
        {
            return ActionGroup.EvaConstructionModeControls;
        }

        public void Start()
        {
            LOGGER.LogInfo("Start");
            GameEvents.OnEVAConstructionMode.Add(OnEVAConstructionModeChanged);
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            this.FireContextEnterOrLeave(false);
        }

        public void OnDestroy()
        {
            LOGGER.LogInfo("OnDestroy");
            GameEvents.OnEVAConstructionMode.Remove(OnEVAConstructionModeChanged);
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        // ============================================================

        protected void OnSceneUnloaded(Scene scene)
        {
            LOGGER.LogDebug("OnSceneUnloaded : " + scene.name);
            if( scene.name.ToUpper() != "PFLIGHT4" ) return;

            this.FireContextEnterOrLeave(false);
        }

        protected void OnEVAConstructionModeChanged(bool mode)
        {
            LOGGER.LogTrace("=> OnEVAConstructionModeChanged : " + mode);
            FireContextEnterOrLeave(mode);
        }
    }
}
