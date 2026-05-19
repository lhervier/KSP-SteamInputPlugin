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
    //  This class is a context daemon that detects when the game is in the mission control
    // </summary>
    public class MissionControlCtxDaemon : BaseContextDaemon
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("MissionControlCtxDaemon");

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

            GameEvents.onGUIMissionControlSpawn.Add(OnGUIMissionControlSpawn);
            GameEvents.onGUIMissionControlDespawn.Add(OnGUIMissionControlDespawn);
        }

        protected void OnSceneUnloaded(Scene scene)
        {
            LOGGER.LogDebug("OnSceneUnloaded : " + scene.name);
            if( scene.name.ToUpper() != "SPACECENTER" ) return;
            
            GameEvents.onGUIMissionControlSpawn.Remove(OnGUIMissionControlSpawn);
            GameEvents.onGUIMissionControlDespawn.Remove(OnGUIMissionControlDespawn);
            this.FireContextEnterOrLeave(false);
        }

        protected void OnGUIMissionControlSpawn()
        {
            LOGGER.LogTrace("=> OnGUIMissionControlSpawn");
            this.FireContextEnterOrLeave(true);
        }

        protected void OnGUIMissionControlDespawn()
        {
            LOGGER.LogTrace("=> OnGUIMissionControlDespawn");
            this.FireContextEnterOrLeave(false);
        }       
    }
}