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
    //  This class is a context daemon that detects when the game is in the administration building
    // </summary>
    public class AdministrationBuildingCtxDaemon : BaseContextDaemon
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("AdministrationBuildingCtxDaemon");
        
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
            LOGGER.LogDebug("=> OnSceneLoaded : " + scene.name);
            if( scene.name.ToUpper() != "SPACECENTER" ) return;
            GameEvents.onGUIAdministrationFacilitySpawn.Add(OnGUIAdministrationFacilitySpawn);
            GameEvents.onGUIAdministrationFacilityDespawn.Add(OnGUIAdministrationFacilityDespawn);
        }

        protected void OnSceneUnloaded(Scene scene)
        {
            LOGGER.LogDebug("=> OnSceneUnloaded : " + scene.name);
            if( scene.name.ToUpper() != "SPACECENTER" ) return;
            GameEvents.onGUIAdministrationFacilitySpawn.Remove(OnGUIAdministrationFacilitySpawn);
            GameEvents.onGUIAdministrationFacilityDespawn.Remove(OnGUIAdministrationFacilityDespawn);

            this.FireContextEnterOrLeave(false);
        }

        // ================================================================================================================

        protected void OnGUIAdministrationFacilitySpawn()
        {
            LOGGER.LogTrace("=> OnGUIAdministrationFacilitySpawn");
            this.FireContextEnterOrLeave(true);
        }
        
        protected void OnGUIAdministrationFacilityDespawn()
        {
            LOGGER.LogTrace("=> OnGUIAdministrationFacilityDespawn");
            this.FireContextEnterOrLeave(false);
        }
    }
}