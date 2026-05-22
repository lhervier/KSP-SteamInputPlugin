using System.Collections.Generic;
using UnityEngine;
using com.github.lhervier.ksp;
using com.github.lhervier.ksp.ui.model;
using System.Linq;
using System;

namespace com.github.lhervier.ksp.ui
{
    public class CheatSheetViewModel: MonoBehaviour
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("CheatSheetViewModel");
        
        public string LastConfigError => _lastConfigLoadError;
        private string _lastConfigLoadError = "";

        public bool ShowLoggingIcon
        {
            get => _showLoggingIcon;
            set => SteamInputGlobalSettings.SetShowLoggingIcon(value);
        }
        private bool _showLoggingIcon = false;


        public string ControllerConfigName
        {
            get => _controllerConfigName;
            set => SteamInputGlobalSettings.SetControllerConfigName(value);
        }
        private string _controllerConfigName = "";

        public LogLevel LogLevel
        {
            get => _logLevel;
            set => SteamInputGlobalSettings.SetLogLevel(value);
        }
        private LogLevel _logLevel = LogLevel.Info;
        
        private string _actionGroupLabel = "";
        public string ActionGroupLabel => _actionGroupLabel;
        public EventData<string> OnActionGroupLabelChanged = new EventData<string>("SteamInput.ActionGroupLabelChanged");
        
        public string GamepadLabel => _gamepadLabel;
        private string _gamepadLabel = "";

        public bool GamepadConnected => _gamepadConnected;
        private bool _gamepadConnected = false;

        public List<string> ActivatedContexts => new List<string>(_activatedContexts);
        private List<string> _activatedContexts = new List<string>();
        
        public List<UIPhysicalZone> PhysicalZones => new List<UIPhysicalZone>(_physicalZones);
        private List<UIPhysicalZone> _physicalZones = new List<UIPhysicalZone>();

        private GamepadConfigDaemon _gamepadConfigDaemon;
        private ActionGroupDaemon _actionGroupDaemon;
        private GamepadDaemon _gamepadDaemon;
        private Action _onClose;

        public void Initialize(
            GamepadConfigDaemon gamepadConfigDaemon, 
            ActionGroupDaemon actionGroupDaemon,
            GamepadDaemon gamepadDaemon,
            Action onClose
        )
        {
            this._gamepadConfigDaemon = gamepadConfigDaemon;
            this._actionGroupDaemon = actionGroupDaemon;
            this._gamepadDaemon = gamepadDaemon;
            this._onClose = onClose;
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
            this._gamepadConfigDaemon.OnConfigLoaded.Add(this.OnGamepadConfigLoaded);
            this._gamepadConfigDaemon.OnConfigLoadError.Add(this.OnGamepadConfigLoadError);

            this._actionGroupDaemon.OnActionGroupChanged.Add(this.OnActionGroupChanged);

            this._gamepadDaemon.OnGamepadConnected.Add(this.OnGamepadConnected);
            this._gamepadDaemon.OnGamepadDisconnected.Add(this.OnGamepadDisconnected);

            SteamInputGlobalSettings.OnGlobalSettingsChanged.Add(this.OnGlobalSettingsChanged);

            this.RefreshAll();

            LOGGER.LogInfo("Start: Started");
        }

        public void OnDestroy()
        {
            LOGGER.LogInfo("OnDestroy");
            SteamInputGlobalSettings.OnGlobalSettingsChanged.Remove(this.OnGlobalSettingsChanged);
            if( this._gamepadConfigDaemon != null ) {
                this._gamepadConfigDaemon.OnConfigLoaded.Remove(this.OnGamepadConfigLoaded);
                this._gamepadConfigDaemon.OnConfigLoadError.Remove(this.OnGamepadConfigLoadError);
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

        private void OnGamepadConfigLoaded()
        {
            LOGGER.LogDebug("OnConfigLoaded");
            this.RefreshControllerType();
            this.RefreshActionGroupLabel();
            this.RefreshPhysicalZones();

            this._lastConfigLoadError = string.Empty;
        }

        private void OnGamepadConfigLoadError(string error)
        {
            this._lastConfigLoadError = error ?? string.Empty;
        }

        private void OnGamepadConnected()
        {
            LOGGER.LogDebug("OnGamepadConnected");
            this.RefreshGamepadConnected();
        }

        private void OnGamepadDisconnected()
        {
            LOGGER.LogDebug("OnGamepadDisconnected");
            this.RefreshGamepadConnected();
        }

        private void OnGlobalSettingsChanged(int updateFlags)
        {
            LOGGER.LogDebug("OnGlobalSettingsChanged");
            if( (updateFlags & UpdatedConfiguration.SHOW_LOGGING_ICON) != 0 ) {
                this.RefreshShowLoggingIcon();
            }
            if( (updateFlags & UpdatedConfiguration.CONTROLLER_CONFIG_NAME) != 0 ) {
                this.RefreshControllerConfigName();
            }
            if( (updateFlags & UpdatedConfiguration.LOG_LEVEL) != 0 ) {
                this.RefreshLogLevel();
            }
            if( (updateFlags & UpdatedConfiguration.ORDERED_GAMEPAD_ZONES) != 0 ) {
                this.RefreshPhysicalZones();
            }
        }

        // =======================================================================
        //              Refresh methods
        // =======================================================================

        private void RefreshAll()
        {
            this.RefreshShowLoggingIcon();
            this.RefreshControllerConfigName();
            this.RefreshLogLevel();
            this.RefreshGamepadConnected();
            this.RefreshControllerType();
            this.RefreshActivatedContexts();
            this.RefreshActionGroupLabel();
            this.RefreshPhysicalZones();
        }

        private void RefreshShowLoggingIcon()
        {
            this._showLoggingIcon = SteamInputGlobalSettings.GetShowLoggingIcon();
        }

        private void RefreshControllerConfigName()
        {
            this._controllerConfigName = SteamInputGlobalSettings.GetControllerConfigName();
        }

        private void RefreshLogLevel()
        {
            this._logLevel = SteamInputGlobalSettings.GetLogLevel();
        }

        private void RefreshGamepadConnected()
        {
            this._gamepadConnected = this._gamepadDaemon.GamepadConnected;
        }

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
            }
            else
            {
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
            this.OnActionGroupLabelChanged.Fire(this._actionGroupLabel);
        }

        private void RefreshPhysicalZones()
        {
            this._physicalZones.Clear();
            ActionGroup currentActionGroup = this._actionGroupDaemon.GetCurrentActionGroup();
            List<PhysicalZone> physicalZones = this._gamepadConfigDaemon.GetPhysicalZones(currentActionGroup);
            foreach( GamepadZone zone in SteamInputGlobalSettings.GetOrderedGamepadZones() ) {
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

        public void CloseWindow()
        {
            this._onClose?.Invoke();
        }
    }
}