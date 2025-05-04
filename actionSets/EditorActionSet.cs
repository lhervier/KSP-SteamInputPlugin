using UnityEngine;
using System;
using System.Collections;

namespace com.github.lhervier.ksp 
{
    public class EditorActionSet : MonoBehaviour, IKspActionSet {
        private static readonly string modeName = "EditorControls";
        private static SteamControllerLogger LOGGER = new SteamControllerLogger(modeName);

        public string ControlName() {
            return modeName;
        }

        public bool Active() {
            return HighLogic.LoadedSceneIsEditor;
        }
    }
}