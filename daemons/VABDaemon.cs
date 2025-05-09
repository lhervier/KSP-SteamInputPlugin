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
    public class VABDaemon : ControllerContextDaemon
    {
        private static readonly SteamControllerLogger LOGGER = new SteamControllerLogger("VABDaemon");
        protected override string ActionGroupName()
        {
            return "EditorControls";
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
            if( !scene.name.ToUpper().StartsWith("VAB") ) {
                return;
            }
            this.SendEvent(true);
        }

        protected void OnSceneUnloaded(Scene scene)
        {
            LOGGER.Log("OnSceneUnloaded : " + scene.name);
            if( !scene.name.ToUpper().StartsWith("VAB") ) {
                return;
            }
            this.SendEvent(false);
        }
    }
}