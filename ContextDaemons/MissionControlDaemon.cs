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
    public class MissionControlDaemon : BaseContextDaemon
    {
        private static readonly SteamControllerLogger LOGGER = new SteamControllerLogger("MissionControlDaemon");

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

            GameEvents.onGUIMissionControlSpawn.Add(OnGUIMissionControlSpawn);
            GameEvents.onGUIMissionControlDespawn.Add(OnGUIMissionControlDespawn);
        }

        protected void OnSceneUnloaded(Scene scene)
        {
            // LOGGER.Log("OnSceneUnloaded : " + scene.name);
            if( scene.name.ToUpper() != "SPACECENTER" ) return;
            
            GameEvents.onGUIMissionControlSpawn.Remove(OnGUIMissionControlSpawn);
            GameEvents.onGUIMissionControlDespawn.Remove(OnGUIMissionControlDespawn);
        }

        protected void OnGUIMissionControlSpawn()
        {
            // LOGGER.Log("=> OnGUIMissionControlSpawn");
            this.FireContextEnterOrLeave(true);
        }

        protected void OnGUIMissionControlDespawn()
        {
            // LOGGER.Log("=> OnGUIMissionControlDespawn");
            this.FireContextEnterOrLeave(false);
        }       
    }
}