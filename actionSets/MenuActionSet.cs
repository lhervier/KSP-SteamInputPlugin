using UnityEngine;
using System;
using System.Collections;

namespace com.github.lhervier.ksp 
{
    public class MenuActionSet : MonoBehaviour, IKspActionSet {
        private static readonly string controlName = "MenuControls";
        private static SteamControllerLogger LOGGER = new SteamControllerLogger(controlName);

        public string ControlName() {
            return controlName;
        }

        public RefreshType Active() {
            // System screens and menus
            if( HighLogic.LoadedScene == GameScenes.MAINMENU ) {
                return RefreshType.Immediate;
            }
            
            // In flight, game paused
            if( HighLogic.LoadedSceneIsFlight && SteamControllerPlugin.GamePaused ) 
            {
                return RefreshType.Immediate;
            }

            return RefreshType.Nope;
        }

        public bool Default() {
            return true;
        }
    }
}