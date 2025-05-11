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
    //  This class is a context daemon that detects when the game is in the astronaut complex
    // </summary>
    public class AstronautComplexCtxDaemon : BaseContextDaemon
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("AstronautComplexCtxDaemon");

        public override ActionGroup CorrespondingActionGroup()
        {
            return ActionGroup.MenuControls;
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
            if( scene.name.ToUpper() != "SPACECENTER" ) return;

            GameEvents.onGUIAstronautComplexSpawn.Add(OnGUIAstronautComplexSpawn);
            GameEvents.onGUIAstronautComplexDespawn.Add(OnGUIAstronautComplexDespawn);
        }

        protected void OnSceneUnloaded(Scene scene)
        {
            LOGGER.LogDebug("OnSceneUnloaded : " + scene.name);
            if( scene.name.ToUpper() != "SPACECENTER" ) return;

            GameEvents.onGUIAstronautComplexSpawn.Remove(OnGUIAstronautComplexSpawn);
            GameEvents.onGUIAstronautComplexDespawn.Remove(OnGUIAstronautComplexDespawn);
        }

        // ==========================================================================

        protected void OnGUIAstronautComplexSpawn()
        {
            LOGGER.LogTrace("=> OnGUIAstronautComplexSpawn");    
            this.FireContextEnterOrLeave(true);
        }

        protected void OnGUIAstronautComplexDespawn()
        {
            LOGGER.LogTrace("=> OnGUIAstronautComplexDespawn");
            this.FireContextEnterOrLeave(false);
        }
    }
}
