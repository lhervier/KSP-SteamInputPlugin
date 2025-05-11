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
        private static readonly SteamControllerLogger LOGGER = new SteamControllerLogger("EVAConstructionModeCtxDaemon");

        public override ActionGroup CorrespondingActionGroup()
        {
            return ActionGroup.EvaConstructionModeControls;
        }

        public void Start()
        {
            LOGGER.LogInfo("Start");
            GameEvents.OnEVAConstructionMode.Add(OnEVAConstructionModeChanged);

            this.FireContextEnterOrLeave(false);
        }

        public void OnDestroy()
        {
            LOGGER.LogInfo("OnDestroy");
            GameEvents.OnEVAConstructionMode.Remove(OnEVAConstructionModeChanged);
        }

        // ============================================================

        protected void OnEVAConstructionModeChanged(bool mode)
        {
            LOGGER.LogDebug("=> OnEVAConstructionModeChanged : " + mode);
            FireContextEnterOrLeave(mode);
        }
    }
}
