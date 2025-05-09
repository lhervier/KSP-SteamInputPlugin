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
    public class AdministrationBuildingDaemon : ControllerContextDaemon
    {
        private static readonly SteamControllerLogger LOGGER = new SteamControllerLogger("AdministrationBuildingDaemon");
        
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
            LOGGER.Log("=> OnSceneLoaded : " + scene.name);
            if( scene.name.ToUpper() != "SPACECENTER" ) return;
            GameEvents.onGUIAdministrationFacilitySpawn.Add(OnGUIAdministrationFacilitySpawn);
            GameEvents.onGUIAdministrationFacilityDespawn.Add(OnGUIAdministrationFacilityDespawn);
        }

        protected void OnSceneUnloaded(Scene scene)
        {
            LOGGER.Log("=> OnSceneUnloaded : " + scene.name);
            if( scene.name.ToUpper() != "SPACECENTER" ) return;
            GameEvents.onGUIAdministrationFacilitySpawn.Remove(OnGUIAdministrationFacilitySpawn);
            GameEvents.onGUIAdministrationFacilityDespawn.Remove(OnGUIAdministrationFacilityDespawn);
        }

        // ================================================================================================================

        protected void OnGUIAdministrationFacilitySpawn()
        {
            LOGGER.Log("=> OnGUIAdministrationFacilitySpawn");
            this.SendEvent(true, RefreshType.Immediate);
        }
        
        protected void OnGUIAdministrationFacilityDespawn()
        {
            LOGGER.Log("=> OnGUIAdministrationFacilityDespawn");
            this.SendEvent(false, RefreshType.Immediate);
        }
    }
}