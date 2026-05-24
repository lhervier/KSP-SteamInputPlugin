using System.Collections.Generic;
using UnityEngine;
using com.github.lhervier.ksp;
using com.github.lhervier.ksp.ui.model;
using System.Linq;
using System;
using com.github.lhervier.ksp.model;

namespace com.github.lhervier.ksp.ui
{
    public class CheatSheetViewModel: MonoBehaviour
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("CheatSheetViewModel");
        
        // ===================================================
        // The error when loading a gamepad config
        // ===================================================
        public string LastConfigError => _lastConfigLoadError;
        private string _lastConfigLoadError = "";
        public EventData<string> OnConfigLoadError = new EventData<string>("SteamInput.OnConfigLoadError");

        // ===================================================
        // Should we display the logging icon
        // ===================================================
        public bool ShowLoggingIcon
        {
            get => _showLoggingIcon;
            set => SteamInputGlobalSettings.SetShowLoggingIcon(value);
        }
        private bool _showLoggingIcon = false;
        public EventData<bool> OnShowLoggingIconChanged = new EventData<bool>("SteamInput.OnShowLoggingIconChanged");

        // ===================================================
        // The gamepad config name
        // ===================================================
        public string GamepadConfigName
        {
            get => _gamepadConfigName;
            set => SteamInputGlobalSettings.SetControllerConfigName(value);
        }
        private string _gamepadConfigName = "";
        public EventData<string> OnGamepadConfigNameChanged = new EventData<string>("SteamInput.OnGamepadConfigNameChanged");

        // ===================================================
        // The log level
        // ===================================================
        public LogLevel LogLevel
        {
            get => _logLevel;
            set => SteamInputGlobalSettings.SetLogLevel(value);
        }
        private LogLevel _logLevel = LogLevel.Info;
        public EventData<LogLevel> OnLogLevelChanged = new EventData<LogLevel>("SteamInput.OnLogLevelChanged");
        
        // ===================================================
        // The label of the current action group
        // ===================================================
        private string _actionGroupLabel = "";
        public string ActionGroupLabel => _actionGroupLabel;
        public EventData<string> OnActionGroupLabelChanged = new EventData<string>("SteamInput.OnActionGroupLabelChanged");
        
        // ====================================================
        // The label of the loaded gamepad
        // ====================================================
        public string GamepadLabel => _gamepadLabel;
        private string _gamepadLabel = "";
        public EventData<string> OnGamepadLabelChanged = new EventData<string>("SteamInput.OnGamepadLabelChanged");

        // ====================================================
        // Is a gamepad connected ?
        // ====================================================
        public bool GamepadConnected => _gamepadConnected;
        private bool _gamepadConnected = false;
        public EventData<bool> OnGamepadConnected = new EventData<bool>("SteamInput.OnGamepadConnected");

        // ====================================================
        // The active contexts
        // ====================================================
        public List<string> ActivatedContexts => new List<string>(_activatedContexts);
        private List<string> _activatedContexts = new List<string>();
        public EventData<List<string>> OnActivatedContextsChanged = new EventData<List<string>>("SteamInput.OnActiveContextsChanged");
        
        // ====================================================
        // The gamepad zones for the current action group
        // ====================================================
        public List<UIActionGroupZone> ActionGroupZones => new List<UIActionGroupZone>(_actionGroupZones);
        private List<UIActionGroupZone> _actionGroupZones = new List<UIActionGroupZone>();
        public EventData<List<UIActionGroupZone>> OnActionGroupZonesChanged = new EventData<List<UIActionGroupZone>>("SteamInput.OnActionGroupZonesChanged");

        // ==================================================================
        // The gamepad zones défined in the current configuration
        // ==================================================================
        public List<UIConfigZone> ConfigZones => new List<UIConfigZone>(_configZones);
        private List<UIConfigZone> _configZones = new List<UIConfigZone>();
        public EventData<List<UIConfigZone>> OnConfigZonesChanged = new EventData<List<UIConfigZone>>("SteamInput.OnConfigZonesChanged");

        // ==============================================================================================

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
            this._gamepadConfigDaemon.OnConfigLoaded.Add(this._OnGamepadConfigLoaded);
            this._gamepadConfigDaemon.OnConfigLoadError.Add(this._OnGamepadConfigLoadError);

            this._actionGroupDaemon.OnActionGroupChanged.Add(this._OnActionGroupChanged);

            this._gamepadDaemon.OnGamepadConnected.Add(this._OnGamepadConnected);
            this._gamepadDaemon.OnGamepadDisconnected.Add(this._OnGamepadDisconnected);

            SteamInputGlobalSettings.OnGlobalSettingsChanged.Add(this._OnGlobalSettingsChanged);

            this.RefreshAll();

            LOGGER.LogInfo("Start: Started");
        }

        public void OnDestroy()
        {
            LOGGER.LogInfo("OnDestroy");
            SteamInputGlobalSettings.OnGlobalSettingsChanged.Remove(this._OnGlobalSettingsChanged);
            if( this._gamepadConfigDaemon != null ) {
                this._gamepadConfigDaemon.OnConfigLoaded.Remove(this._OnGamepadConfigLoaded);
                this._gamepadConfigDaemon.OnConfigLoadError.Remove(this._OnGamepadConfigLoadError);
            }
            if( this._actionGroupDaemon != null ) {
                this._actionGroupDaemon.OnActionGroupChanged.Remove(this._OnActionGroupChanged);
            }
            if( this._gamepadDaemon != null ) {
                this._gamepadDaemon.OnGamepadConnected.Remove(this._OnGamepadConnected);
                this._gamepadDaemon.OnGamepadDisconnected.Remove(this._OnGamepadDisconnected);
            }
            LOGGER.LogInfo("OnDestroy: Destroyed");
        }

        // =======================================================================

        private void _OnActionGroupChanged(ActionGroup actionGroup)
        {
            LOGGER.LogDebug("OnActionGroupChanged: " + actionGroup.ToString());
            this.RefreshActivatedContexts();
            this.RefreshActionGroupLabel();
            this.RefreshActionGroupZones();
        }

        private void _OnGamepadConfigLoaded()
        {
            LOGGER.LogDebug("OnConfigLoaded");
            this.RefreshControllerType();
            this.RefreshActionGroupLabel();
            this.RefreshConfigZones();
            this.RefreshActionGroupZones();

            this._lastConfigLoadError = string.Empty;
            this.OnConfigLoadError.Fire(_lastConfigLoadError);
        }

        private void _OnGamepadConfigLoadError(string error)
        {
            this._lastConfigLoadError = error ?? string.Empty;
            this.OnConfigLoadError.Fire(_lastConfigLoadError);
        }

        private void _OnGamepadConnected()
        {
            LOGGER.LogDebug("OnGamepadConnected");
            this.RefreshGamepadConnected();
        }

        private void _OnGamepadDisconnected()
        {
            LOGGER.LogDebug("OnGamepadDisconnected");
            this.RefreshGamepadConnected();
        }

        private void _OnGlobalSettingsChanged(int updateFlags)
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
                this.RefreshConfigZones();
                this.RefreshActionGroupZones();
            }
            if( (updateFlags & UpdatedConfiguration.VISIBLE_GAMEPAD_ZONES) != 0 ) {
                this.RefreshConfigZones();
                this.RefreshActionGroupZones();
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
            this.RefreshConfigZones();
            this.RefreshActionGroupZones();
        }

        private void RefreshShowLoggingIcon()
        {
            this._showLoggingIcon = SteamInputGlobalSettings.GetShowLoggingIcon();
            this.OnShowLoggingIconChanged.Fire(this._showLoggingIcon);
        }

        private void RefreshControllerConfigName()
        {
            this._gamepadConfigName = SteamInputGlobalSettings.GetControllerConfigName();
            this.OnGamepadConfigNameChanged.Fire(this._gamepadConfigName);
        }

        private void RefreshLogLevel()
        {
            this._logLevel = SteamInputGlobalSettings.GetLogLevel();
            OnLogLevelChanged.Fire(this._logLevel);
        }

        private void RefreshGamepadConnected()
        {
            this._gamepadConnected = this._gamepadDaemon.GamepadConnected;
            OnGamepadConnected.Fire(this._gamepadConnected);
        }

        private void RefreshControllerType()
        {
            this._gamepadLabel = GamepadControllerTypes.GetDisplayName(
                this._gamepadConfigDaemon.GetControllerType()
            );
            OnGamepadLabelChanged.Fire(this._gamepadLabel);
        }

        private void RefreshActivatedContexts()
        {
            this._activatedContexts.Clear();
            if (this._actionGroupDaemon != null)
            {
                this._activatedContexts.AddRange(this._actionGroupDaemon.ActivatedContexts);
            }
            OnActivatedContextsChanged.Fire(this._activatedContexts);
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

        private string GetLabel(GamepadZone zone)
        {
            return ModLocalization.GetString("SteamInput_physicalZone_" + zone.Name).ToUpperInvariant();
        }

        private List<GamepadZone> GetAllZones()
        {
            List<GamepadZone> gamepadZones = this._gamepadConfigDaemon.GetGamepadZones();
            List<GamepadZone> orderedZones = SteamInputGlobalSettings.GetOrderedGamepadZones();

            // Add all the unknown gamepad zones as hidden zones (at the end of the list)
            foreach( GamepadZone zone in gamepadZones )
            {
                if( !orderedZones.Contains(zone) )
                {
                    orderedZones.Add(zone);
                }
            }
            return orderedZones;
        }

        private void RefreshConfigZones()
        {
            this._configZones.Clear();

            List<GamepadZone> orderedZones = GetAllZones();
            List<GamepadZone> visibleZones = SteamInputGlobalSettings.GetVisibleGamepadZones();
            for( int i=0; i<orderedZones.Count; i++ )
            {
                GamepadZone zone = orderedZones[i];
                this._configZones.Add(
                    new UIConfigZone
                    {
                        Zone = zone,
                        Label = GetLabel(zone),
                        Visible = visibleZones.Contains(zone),
                        First = i == 0,
                        Last = i == orderedZones.Count - 1
                    }
                );
            }

            OnConfigZonesChanged.Fire(this._configZones);
        }

        private void RefreshActionGroupZones()
        {
            this._actionGroupZones.Clear();
            
            ActionGroup currentActionGroup = this._actionGroupDaemon.GetCurrentActionGroup();
            
            List<GamepadZone> orderedZones = GetAllZones();
            List<GamepadZone> visibleZones = SteamInputGlobalSettings.GetVisibleGamepadZones();
            Dictionary<GamepadZone, ActionGroupZone> actionGroupZones = this._gamepadConfigDaemon.GetZones(currentActionGroup);
            for( int i=0; i<orderedZones.Count; i++ )
            {
                GamepadZone zone = orderedZones[i];
                if( !visibleZones.Contains(zone) ) {
                    continue;
                }
                if (!actionGroupZones.TryGetValue(zone, out ActionGroupZone physicalZone))
                {
                    continue;
                }
                this._actionGroupZones.Add(
                    new UIActionGroupZone
                    {
                        Zone = zone,
                        Label = GetLabel(zone),
                        GroupId = physicalZone?.GroupId,
                        ModeshiftGroupId = physicalZone?.ModeshiftGroupId,
                        First = i == 0,
                        Last = i == orderedZones.Count - 1
                    }
                );
            }
            OnActionGroupZonesChanged.Fire(this._actionGroupZones);
        }

        // =======================================================================

        public void CloseWindow()
        {
            this._onClose?.Invoke();
        }

        public void MoveZoneUp(UIConfigZone zone)
        {
            List<GamepadZone> zones = SteamInputGlobalSettings.GetOrderedGamepadZones();
            int index = zones.IndexOf(zone.Zone);
            if( index == -1 ) return;
            if( index == 0 ) return;
            
            (zones[index - 1], zones[index]) = (zones[index], zones[index - 1]);
            SteamInputGlobalSettings.SetOrderedGamepadZones(zones);
        }

        public void MoveZoneDown(UIConfigZone zone)
        {
            List<GamepadZone> zones = SteamInputGlobalSettings.GetOrderedGamepadZones();
            int index = zones.IndexOf(zone.Zone);
            if( index == -1 ) return;
            if( index == zones.Count - 1 ) return;
            
            (zones[index], zones[index + 1]) = (zones[index + 1], zones[index]);
            SteamInputGlobalSettings.SetOrderedGamepadZones(zones);
        }

        public void ToggleZoneVisibility(UIConfigZone zone)
        {
            List<GamepadZone> visibleZones = SteamInputGlobalSettings.GetVisibleGamepadZones();
            if( visibleZones.Contains(zone.Zone) )
            {
                visibleZones.Remove(zone.Zone);
            } else
            {
                visibleZones.Add(zone.Zone);
            }
            SteamInputGlobalSettings.SetVisibleGamepadZones(visibleZones);
        }
    }
}