using UnityEngine;
using System;
using System.Collections;

namespace com.github.lhervier.ksp 
{
    public class EditorActionSet : MonoBehaviour, IKspActionSet {
        private static readonly string controlName = "EditorControls";
        private static SteamControllerLogger LOGGER = new SteamControllerLogger(controlName);

        public string ControlName() {
            return controlName;
        }

        public bool Active() {
            return HighLogic.LoadedSceneIsEditor;
        }

        public bool Default() {
            return false;
        }
    }
}