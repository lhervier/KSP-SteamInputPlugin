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
    //  This class is a context daemon that detects when the game is in the RnD complex
    // </summary>
    public class RnDComplexCtxDaemon : BaseContextDaemon
    {
        private static readonly SteamControllerLogger LOGGER = new SteamControllerLogger("RnDComplexCtxDaemon");
        
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

            GameEvents.onGUIRnDComplexSpawn.Add(OnGUIRnDComplexSpawn);
            GameEvents.onGUIRnDComplexDespawn.Add(OnGUIRnDComplexDespawn);
        }

        protected void OnSceneUnloaded(Scene scene)
        {
            LOGGER.LogDebug("OnSceneUnloaded : " + scene.name);
            if( scene.name.ToUpper() != "SPACECENTER" ) return;

            GameEvents.onGUIRnDComplexSpawn.Remove(OnGUIRnDComplexSpawn);
            GameEvents.onGUIRnDComplexDespawn.Remove(OnGUIRnDComplexDespawn);
        }

        protected void OnGUIRnDComplexSpawn()
        {
            LOGGER.LogDebug("=> OnGUIRnDComplexSpawn");
            this.FireContextEnterOrLeave(true);
        }
        
        protected void OnGUIRnDComplexDespawn()
        {
            LOGGER.LogDebug("=> OnGUIRnDComplexDespawn");
            this.FireContextEnterOrLeave(false);
        }
    }
}
