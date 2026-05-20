using System.Collections.Generic;
using UnityEngine;
using com.github.lhervier.ksp;
using com.github.lhervier.ksp.ui.model;
using System.Linq;

namespace com.github.lhervier.ksp.ui
{
    public class CheatSheetViewModel: MonoBehaviour
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("CheatSheetViewModel");
        
        private string _lastError = "";
        private string _actionGroupLabel = "";
        private string _gamepadLabel = "";
        private bool _gamepadConnected = false;
        private List<string> _activatedContexts = new List<string>();
        private List<UIPhysicalZone> _physicalZones = new List<UIPhysicalZone>();

        private GamepadConfigDaemon _gamepadConfigDaemon;
        private ActionGroupDaemon _actionGroupDaemon;
        private GamepadDaemon _gamepadDaemon;

        public void Initialize(
            GamepadConfigDaemon gamepadConfigDaemon, 
            ActionGroupDaemon actionGroupDaemon,
            GamepadDaemon gamepadDaemon
        )
        {
            this._gamepadConfigDaemon = gamepadConfigDaemon;
            this._actionGroupDaemon = actionGroupDaemon;
            this._gamepadDaemon = gamepadDaemon;
        }

        public void Awake()
        {
            LOGGER.LogInfo("Awake");
            DontDestroyOnLoad(this);
        }

        public void Start()
        {
            LOGGER.LogInfo("Start");
            if( this._gamepadConfigDaemon == null
                || this._actionGroupDaemon == null
                || this._gamepadDaemon == null ) {
                LOGGER.LogError("Start: ViewModel dependencies not initialized");
                return;
            }
            this._gamepadConfigDaemon.OnConfigLoaded.Add(this.OnConfigLoaded);
            this._gamepadConfigDaemon.OnConfigLoadError.Add(this.OnConfigLoadError);

            this._actionGroupDaemon.OnActionGroupChanged.Add(this.OnActionGroupChanged);

            this._gamepadDaemon.OnGamepadConnected.Add(this.OnGamepadConnected);
            this._gamepadDaemon.OnGamepadDisconnected.Add(this.OnGamepadDisconnected);

            SteamInputGlobalSettings.OnConfigurationChanged.Add(this.OnConfigurationChanged);

            this._gamepadConnected = this._gamepadDaemon.GamepadConnected;
            this.RefreshActivatedContexts();
            this.RefreshControllerType();
            this.RefreshPhysicalZones();
            LOGGER.LogInfo("Start: Started");
        }

        public void OnDestroy()
        {
            LOGGER.LogInfo("OnDestroy");
            SteamInputGlobalSettings.OnConfigurationChanged.Remove(this.OnConfigurationChanged);
            if( this._gamepadConfigDaemon != null ) {
                this._gamepadConfigDaemon.OnConfigLoaded.Remove(this.OnConfigLoaded);
                this._gamepadConfigDaemon.OnConfigLoadError.Remove(this.OnConfigLoadError);
            }
            if( this._actionGroupDaemon != null ) {
                this._actionGroupDaemon.OnActionGroupChanged.Remove(this.OnActionGroupChanged);
            }
            if( this._gamepadDaemon != null ) {
                this._gamepadDaemon.OnGamepadConnected.Remove(this.OnGamepadConnected);
                this._gamepadDaemon.OnGamepadDisconnected.Remove(this.OnGamepadDisconnected);
            }
            LOGGER.LogInfo("OnDestroy: Destroyed");
        }

        // =======================================================================

        private void OnActionGroupChanged(ActionGroup actionGroup)
        {
            LOGGER.LogDebug("OnActionGroupChanged: " + actionGroup.ToString());
            this.RefreshActivatedContexts();
            this.RefreshActionGroupLabel();
            this.RefreshPhysicalZones();
        }

        private void OnConfigLoaded()
        {
            LOGGER.LogDebug("OnConfigLoaded");
            this.RefreshControllerType();
            this.RefreshActionGroupLabel();
            this.RefreshPhysicalZones();
            this._lastError = string.Empty;
        }

        private void OnConfigLoadError(string error)
        {
            this._lastError = error ?? string.Empty;
        }

        private void OnGamepadConnected()
        {
            LOGGER.LogDebug("OnGamepadConnected");
            this._gamepadConnected = true;
        }

        private void OnGamepadDisconnected()
        {
            LOGGER.LogDebug("OnGamepadDisconnected");
            this._gamepadConnected = false;
        }

        private void OnConfigurationChanged()
        {
            LOGGER.LogDebug("OnConfigurationChanged");
            this.RefreshControllerType();
            this.RefreshActionGroupLabel();
            this.RefreshPhysicalZones();
        }

        // =======================================================================

        private void RefreshControllerType()
        {
            this._gamepadLabel = GamepadControllerTypes.GetDisplayName(
                this._gamepadConfigDaemon.GetControllerType()
            );
        }

        private void RefreshActivatedContexts()
        {
            this._activatedContexts.Clear();
            if (this._actionGroupDaemon != null)
            {
                this._activatedContexts.AddRange(this._actionGroupDaemon.ActivatedContexts);
            }
        }

        private void RefreshActionGroupLabel()
        {
            ActionGroup currentActionGroup = this._actionGroupDaemon.GetCurrentActionGroup();
            if( currentActionGroup == ActionGroup.None )
            {
                this._actionGroupLabel = "—";
                return;
            }
            Dictionary<string, object> actionData = this._gamepadConfigDaemon.GetAction(currentActionGroup);
            if( actionData.TryGetValue("title", out object title) && title is string titleString )
            {
                this._actionGroupLabel = titleString.ToUpperInvariant();
            }
            else
            {
                this._actionGroupLabel = currentActionGroup.ToString().ToUpperInvariant();
            }
        }

        private void RefreshPhysicalZones()
        {
            this._physicalZones.Clear();
            ActionGroup currentActionGroup = this._actionGroupDaemon.GetCurrentActionGroup();
            List<PhysicalZone> physicalZones = this._gamepadConfigDaemon.GetPhysicalZones(currentActionGroup);
            foreach( GamepadZone zone in SteamInputGlobalSettings.GetPhysicalZones() ) {
                PhysicalZone physicalZone = physicalZones.FirstOrDefault(z => z.Zone == zone);
                if( physicalZone == null ) {
                    continue;
                }
                this._physicalZones.Add(
                    new UIPhysicalZone {
                        Zone = physicalZone.Zone,
                        Label = ModLocalization.GetString("SteamInput_physicalZone_" + physicalZone.Zone.Name).ToUpperInvariant(),
                        GroupId = physicalZone.GroupId,
                        ModeshiftGroupId = physicalZone.ModeshiftGroupId
                    }
                );
            }
        }

        // =======================================================================

        public string getLastError()
        {
            return this._lastError;
        }

        public bool GetGamepadConnected()
        {
            return this._gamepadConnected;
        }

        public List<string> GetActivatedContexts()
        {
            return this._activatedContexts;
        }

        public string GetActionGroupLabel()
        {
            return this._actionGroupLabel;
        }

        public string GetGamepadLabel()
        {
            return this._gamepadLabel;
        }

        public List<UIPhysicalZone> GetPhysicalZones()
        {
            return this._physicalZones;
        }
    }
}