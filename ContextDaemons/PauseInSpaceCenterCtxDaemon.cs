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
    //  This class is a context daemon that detects when the game is paused in the space center
    // </summary>
    public class PauseInSpaceCenterCtxDaemon : BaseContextDaemon
    {
        private static readonly SteamControllerLogger LOGGER = new SteamControllerLogger("PauseInSpaceCenterCtxDaemon");
        private static readonly int DELAY = 10;

        private DelayedActionDaemon delayedActionDaemon;

        public override ActionGroup CorrespondingActionGroup()
        {
            return ActionGroup.MenuControls;
        }

        public void Start()
        {
            LOGGER.Log("Start");

            this.delayedActionDaemon = gameObject.AddComponent<DelayedActionDaemon>();

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        public void OnDestroy()
        {
            LOGGER.Log("OnDestroy");

            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;

            Destroy(this.delayedActionDaemon);
        }

        protected void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // LOGGER.Log("OnSceneLoaded : " + scene.name);
            if( scene.name.ToUpper() != "SPACECENTER" ) return;

            GameEvents.onGUIAdministrationFacilitySpawn.Add(OnGUIAdministrationFacilitySpawn);
            GameEvents.onGUIAstronautComplexSpawn.Add(OnGUIAstronautComplexSpawn);
            GameEvents.onGUIMissionControlSpawn.Add(OnGUIMissionControlSpawn);
            GameEvents.onGUIRnDComplexSpawn.Add(OnGUIRnDComplexSpawn);
            
            GameEvents.onGamePause.Add(OnGamePause);
            GameEvents.onGameUnpause.Add(OnGameUnpause);
        }

        protected void OnSceneUnloaded(Scene scene)
        {
            // LOGGER.Log("OnSceneUnloaded : " + scene.name);
            if( scene.name.ToUpper() != "SPACECENTER" ) return;

            GameEvents.onGUIAdministrationFacilitySpawn.Remove(OnGUIAdministrationFacilitySpawn);
            GameEvents.onGUIAstronautComplexSpawn.Remove(OnGUIAstronautComplexSpawn);
            GameEvents.onGUIMissionControlSpawn.Remove(OnGUIMissionControlSpawn);
            GameEvents.onGUIRnDComplexSpawn.Remove(OnGUIRnDComplexSpawn);

            GameEvents.onGamePause.Remove(OnGamePause);
            GameEvents.onGameUnpause.Remove(OnGameUnpause);
        }

        private void Pause() {
            // LOGGER.Log("   Pause: true");
            this.FireContextEnterOrLeave(true);
        }

        // ================================================================================================================
        
        protected void OnGamePause()
        {
            // LOGGER.Log("=> Game pause asked");
            this.delayedActionDaemon.TriggerDelayedAction(
                Pause, 
                DELAY
            );
        }
        
        private void OnGameUnpause()
        {
            // LOGGER.Log("=> Game unpaused");
            this.FireContextEnterOrLeave(false);
        }

        protected void OnGUIAdministrationFacilitySpawn() {
            // LOGGER.Log("=> OnGUIAdministrationFacilitySpawn: Cancelling pause");
            this.delayedActionDaemon.CancelDelayedAction(Pause);
        }

        protected void OnGUIAstronautComplexSpawn() {
            // LOGGER.Log("=> OnGUIAstronautComplexSpawn: Cancelling pause");
            this.delayedActionDaemon.CancelDelayedAction(Pause);
        }

        protected void OnGUIMissionControlSpawn() {
            // LOGGER.Log("=> OnGUIMissionControlSpawn: Cancelling pause");
            this.delayedActionDaemon.CancelDelayedAction(Pause);
        }

        protected void OnGUIRnDComplexSpawn() {
            // LOGGER.Log("=> OnGUIRnDComplexSpawn: Cancelling pause");
            this.delayedActionDaemon.CancelDelayedAction(Pause);
        }
    }
}
