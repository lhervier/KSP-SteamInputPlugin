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
    public class EVAConstructionModeDaemon : ControllerContextDaemon
    {
        private static readonly SteamControllerLogger LOGGER = new SteamControllerLogger("EVAConstructionModeDaemon");
        public void Start()
        {
            LOGGER.Log("Start");
            GameEvents.OnEVAConstructionMode.Add(OnEVAConstructionModeChanged);

            this.SendEvent(false);
        }

        public void OnDestroy()
        {
            LOGGER.Log("OnDestroy");
            GameEvents.OnEVAConstructionMode.Remove(OnEVAConstructionModeChanged);
        }

        // ============================================================

        protected void OnEVAConstructionModeChanged(bool mode)
        {
            LOGGER.Log("=> OnEVAConstructionModeChanged : " + mode);
            SendEvent(mode);
        }
    }
}
