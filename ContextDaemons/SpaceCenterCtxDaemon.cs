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
    //  This class is a context daemon that detects when the game is in the space center
    // </summary>
    public class SpaceCenterCtxDaemon : BaseContextDaemon
    {
        private static readonly SteamControllerLogger LOGGER = new SteamControllerLogger("SpaceCenterCtxDaemon");

        public override ActionGroup CorrespondingActionGroup()
        {
            return ActionGroup.MenuControls;
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
            // LOGGER.Log("OnSceneLoaded : " + scene.name);
            if( scene.name.ToUpper() != "SPACECENTER" ) return;

            this.FireContextEnterOrLeave(true);

            GameEvents.onGamePause.Add(OnGamePause);
            GameEvents.onGameUnpause.Add(OnGameUnpause);
        }

        protected void OnSceneUnloaded(Scene scene)
        {
            // LOGGER.Log("OnSceneUnloaded : " + scene.name);
            if( scene.name.ToUpper() != "SPACECENTER" ) return;

            GameEvents.onGamePause.Remove(OnGamePause);
            GameEvents.onGameUnpause.Remove(OnGameUnpause);

            this.FireContextEnterOrLeave(false);
        }

        // ============================================================

        protected void OnGamePause()
        {
            // LOGGER.Log("=> OnGamePause");
            this.FireContextEnterOrLeave(false);
        }

        protected void OnGameUnpause()
        {
            // LOGGER.Log("=> OnGameUnpause");
            this.FireContextEnterOrLeave(true);
        }
    }
}
