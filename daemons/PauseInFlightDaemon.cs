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
    public class PauseInFlightDaemon : ControllerContextDaemon
    {
        private static readonly SteamControllerLogger LOGGER = new SteamControllerLogger("PauseInFlightDaemon");

        protected override string ActionGroupName()
        {
            return "MenuControls";
        }
        public void Start()
        {
            LOGGER.Log("Start");

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        public void OnDestroy()
        {
            LOGGER.Log("OnDestroy");

            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        protected void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            LOGGER.Log("OnSceneLoaded : " + scene.name);
            if( scene.name.ToUpper() != "PFLIGHT4" ) return;

            GameEvents.onGamePause.Add(OnGamePause);
            GameEvents.onGameUnpause.Add(OnGameUnpause);
        }

        protected void OnSceneUnloaded(Scene scene)
        {
            LOGGER.Log("OnSceneUnloaded : " + scene.name);
            if( scene.name.ToUpper() != "PFLIGHT4" ) return;

            GameEvents.onGamePause.Remove(OnGamePause);
            GameEvents.onGameUnpause.Remove(OnGameUnpause);
        }
        
        private void OnGamePause()
        {
            LOGGER.Log("=> Game paused");
            this.SendEvent(true);
        }
        
        private void OnGameUnpause()
        {
            LOGGER.Log("=> Game unpaused");
            this.SendEvent(false);
        }
    }
}
