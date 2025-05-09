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
    public class FreeIVADaemon : ControllerContextDaemon
    {
        private static readonly SteamControllerLogger LOGGER = new SteamControllerLogger("FreeIVADaemon");
        protected override string ActionGroupName()
        {
            return "FreeIVAControls";
        }
        
        private Type kerbalIvaAddonType;
        PropertyInfo instanceProperty;
        PropertyInfo buckledProperty;
        private bool initialized = false;
        
        protected void Start() 
        {   
            LOGGER.Log("Starting");
            kerbalIvaAddonType = Type.GetType("FreeIva.KerbalIvaAddon, FreeIva");
            if (kerbalIvaAddonType == null) {
                LOGGER.Log("=> FreeIva mod not found");
                return;
            }
            
            instanceProperty = kerbalIvaAddonType.GetProperty("Instance");
            if (instanceProperty == null) {
                LOGGER.Log("=> Instance property not found. FreeIVA mod has probably evolved...");
                return;
            }
            
            buckledProperty = kerbalIvaAddonType.GetProperty("buckled");
            if (buckledProperty == null) {
                LOGGER.Log("=> buckled property not found. FreeIVA mod has probably evolved...");
                return;
            }

            initialized = true;
        }

        protected void OnDestroy() 
        {
            LOGGER.Log("OnDestroy");
            if( !initialized ) {
                return;
            }
        }

        protected void FixedUpdate()
        {
            if( !initialized ) {
                return;
            }
            
            object instance = instanceProperty.GetValue(null);
            if( instance == null ) {
                return;
            }

            this.SendEvent(
                (bool) buckledProperty.GetValue(instance)
            );
        }
    }
}