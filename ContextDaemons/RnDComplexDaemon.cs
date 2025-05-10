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
    public class RnDComplexDaemon : BaseContextDaemon
    {
        private static readonly SteamControllerLogger LOGGER = new SteamControllerLogger("RnDComplexDaemon");
        
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

            GameEvents.onGUIRnDComplexSpawn.Add(OnGUIRnDComplexSpawn);
            GameEvents.onGUIRnDComplexDespawn.Add(OnGUIRnDComplexDespawn);
        }

        protected void OnSceneUnloaded(Scene scene)
        {
            // LOGGER.Log("OnSceneUnloaded : " + scene.name);
            if( scene.name.ToUpper() != "SPACECENTER" ) return;

            GameEvents.onGUIRnDComplexSpawn.Remove(OnGUIRnDComplexSpawn);
            GameEvents.onGUIRnDComplexDespawn.Remove(OnGUIRnDComplexDespawn);
        }

        protected void OnGUIRnDComplexSpawn()
        {
            // LOGGER.Log("=> OnGUIRnDComplexSpawn");
            this.FireContextEnterOrLeave(true);
        }
        
        protected void OnGUIRnDComplexDespawn()
        {
            // LOGGER.Log("=> OnGUIRnDComplexDespawn");
            this.FireContextEnterOrLeave(false);
        }
    }
}
