using System.Collections.Generic;
using UnityEngine;

namespace com.github.lhervier.ksp.ui
{
    public class CheatSheetViewModel: MonoBehaviour
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("CheatSheetViewModel");

        private Dictionary<string, string> _actionNames = new Dictionary<string, string>();
        private string _controllerType = "";
        private string _lastError = "";

        public string ControllerVdfPathBuffer { get; private set; }

        private GamepadConfigDaemon _gamepadConfigDaemon;

        public void Initialize(GamepadConfigDaemon gamepadConfigDaemon)
        {
            this._gamepadConfigDaemon = gamepadConfigDaemon;
        }

        public void Awake()
        {
            LOGGER.LogInfo("Awake");
            DontDestroyOnLoad(this);
        }

        public void Start()
        {
            LOGGER.LogInfo("Start");
            if( this._gamepadConfigDaemon == null ) {
                LOGGER.LogError("Start: GamepadConfigDaemon not initialized");
                return;
            }
            this._gamepadConfigDaemon.OnConfigLoaded.Add(this.OnConfigLoaded);
            this._gamepadConfigDaemon.OnConfigLoadError.Add(this.OnConfigLoadError);
            this.OnConfigLoaded();
            LOGGER.LogInfo("Start: Started");
        }

        public void OnDestroy()
        {
            LOGGER.LogInfo("OnDestroy");
            if( this._gamepadConfigDaemon != null ) {
                this._gamepadConfigDaemon.OnConfigLoaded.Remove(this.OnConfigLoaded);
                this._gamepadConfigDaemon.OnConfigLoadError.Remove(this.OnConfigLoadError);
            }
            LOGGER.LogInfo("OnDestroy: Destroyed");
        }

        // =======================================================================

        private void OnConfigLoaded()
        {
            LOGGER.LogInfo("OnConfigLoaded");
            if( this._gamepadConfigDaemon == null ) {
                return;
            }
            this._controllerType = this._gamepadConfigDaemon.GetControllerType();

            this._actionNames.Clear();
            Dictionary<string, object> actions = this._gamepadConfigDaemon.GetActions();
            foreach (var action in actions)
            {
                if( action.Value is Dictionary<string, object> actionData ) 
                {
                    if( actionData.TryGetValue("title", out object title) ) {
                        if( title is string titleString ) {
                            this._actionNames[action.Key] = titleString;
                        }
                    }
                }
            }

            this._lastError = _gamepadConfigDaemon.LastError;
        }

        private void OnConfigLoadError(string error)
        {
            this._lastError = error;
        }

        // =======================================================================

        public string getLastError()
        {
            return this._lastError;
        }

        public string GetActionGroupLabel(ActionGroup actionGroup)
        {
            if( actionGroup == ActionGroup.None ) {
                return "—";
            }
            if (this._actionNames.TryGetValue(actionGroup.ToString(), out string actionGroupLabel)) {
                return actionGroupLabel.ToUpperInvariant();
            }
            return actionGroup.ToString().ToUpperInvariant();
        }
    }
}