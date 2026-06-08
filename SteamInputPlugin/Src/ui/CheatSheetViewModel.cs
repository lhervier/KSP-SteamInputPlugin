using System.Collections.Generic;
using UnityEngine;
using com.github.lhervier.ksp.steaminput;
using com.github.lhervier.ksp.steaminput.ui.model;
using System.Linq;
using System;
using com.github.lhervier.ksp.steaminput.model;
using System.Text.RegularExpressions;

namespace com.github.lhervier.ksp.steaminput.ui
{
    public class CheatSheetViewModel: MonoBehaviour
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("CheatSheetViewModel");
        
        private readonly DictionaryValueList<string, VdfGroup> _groupsCache = new DictionaryValueList<string, VdfGroup>();

        // Group modes that drive the pointer with free movement (mockup .kmouse-line).
        private static readonly HashSet<string> MouseModes = new HashSet<string>
        {
            "joystick_mouse",
            "absolute_mouse",
        };

        // ===================================================
        // The error when loading a gamepad config
        // ===================================================
        public string LastConfigError => _lastConfigLoadError;
        private string _lastConfigLoadError = "";
        public EventData<string> OnConfigLoadError = new EventData<string>("SteamInput.OnConfigLoadError");

        // ===================================================
        // The gamepad config name
        // ===================================================
        public string GamepadConfigName
        {
            get => _gamepadConfigName;
            set => SteamInputSettings.SetControllerConfigName(value);
        }
        private string _gamepadConfigName = "";
        public EventData<string> OnGamepadConfigNameChanged = new EventData<string>("SteamInput.OnGamepadConfigNameChanged");

        // ===================================================
        // The configs available in the Steam config folder
        // ===================================================
        public List<UIGamepadConfig> Configs => new List<UIGamepadConfig>(_configs);
        private List<UIGamepadConfig> _configs = new List<UIGamepadConfig>();
        public EventData<List<UIGamepadConfig>> OnConfigsChanged = new EventData<List<UIGamepadConfig>>("SteamInput.OnConfigsChanged");

        // ===================================================
        // The log level
        // ===================================================
        public LogLevel LogLevel
        {
            get => _logLevel;
            set => SteamInputSettings.SetLogLevel(value);
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
        public List<UIPhysicalZone> PhysicalZones => new List<UIPhysicalZone>(_physicalZones);
        private List<UIPhysicalZone> _physicalZones = new List<UIPhysicalZone>();
        public EventData<List<UIPhysicalZone>> OnPhysicalZonesChanged = new EventData<List<UIPhysicalZone>>("SteamInput.OnPresetZonesChanged");

        // ==================================================================
        // The gamepad zones défined in the current configuration
        // ==================================================================
        public List<UIConfigZone> ConfigZones => new List<UIConfigZone>(_configZones);
        private List<UIConfigZone> _configZones = new List<UIConfigZone>();
        public EventData<List<UIConfigZone>> OnConfigZonesChanged = new EventData<List<UIConfigZone>>("SteamInput.OnConfigZonesChanged");

        // ==================================================================
        // Is the menu actually displayed ?
        // ==================================================================
        public bool MenuDisplayed => _menuDisplayed;
        private bool _menuDisplayed = false;
        public EventData<bool> OnShowMenu = new EventData<bool>("SteamInput.OnShowMenu");

        // ==================================================================
        // Are the settings actually displayed ?
        // ==================================================================
        public bool SettingsDisplayed => _settingsDisplayed;
        private bool _settingsDisplayed = false;
        public EventData<bool> OnShowSettings = new EventData<bool>("SteamInput.OnShowSettings");

        // ==============================================================================================

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
            this._gamepadConfigDaemon.OnConfigLoaded.Add(this._OnGamepadConfigLoaded);
            this._gamepadConfigDaemon.OnConfigLoadError.Add(this._OnGamepadConfigLoadError);

            this._gamepadConfigDaemon.OnConfigsAvailable.Add(this._OnGamepadConfigsAvailable);

            this._actionGroupDaemon.OnActionGroupChanged.Add(this._OnActionGroupChanged);

            this._gamepadDaemon.OnGamepadConnected.Add(this._OnGamepadConnected);
            this._gamepadDaemon.OnGamepadDisconnected.Add(this._OnGamepadDisconnected);

            SteamInputSettings.OnGlobalSettingsChanged.Add(this._OnGlobalSettingsChanged);

            this.RefreshAll();

            LOGGER.LogInfo("Start: Started");
        }

        public void OnDestroy()
        {
            LOGGER.LogInfo("OnDestroy");
            SteamInputSettings.OnGlobalSettingsChanged.Remove(this._OnGlobalSettingsChanged);
            if( this._gamepadConfigDaemon != null ) {
                this._gamepadConfigDaemon.OnConfigLoaded.Remove(this._OnGamepadConfigLoaded);
                this._gamepadConfigDaemon.OnConfigLoadError.Remove(this._OnGamepadConfigLoadError);
                this._gamepadConfigDaemon.OnConfigsAvailable.Remove(this._OnGamepadConfigsAvailable);
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

        private void _OnActionGroupChanged(EActionGroup actionGroup)
        {
            LOGGER.LogDebug("OnActionGroupChanged: " + actionGroup.ToString());
            this.RefreshActivatedContexts();
            this.RefreshActionGroupLabel();
            this.RefreshPhysicalZones();
        }

        private void _OnGamepadConfigLoaded()
        {
            LOGGER.LogDebug("OnConfigLoaded");
            this.RefreshControllerType();
            this.RefreshActionGroupLabel();
            this.RefreshConfigZones();
            this.RefreshPhysicalZones();

            this._lastConfigLoadError = string.Empty;
            this._groupsCache.Clear();
            this.OnConfigLoadError.Fire(_lastConfigLoadError);
        }

        private void _OnGamepadConfigLoadError(string error)
        {
            this._lastConfigLoadError = error ?? string.Empty;
            this.OnConfigLoadError.Fire(_lastConfigLoadError);
        }

        private void _OnGamepadConfigsAvailable()
        {
            LOGGER.LogDebug("OnGamepadConfigsAvailable");
            this.RefreshGamepadConfigs();
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
            if( (updateFlags & UpdatedConfiguration.CONTROLLER_CONFIG_NAME) != 0 ) {
                this.RefreshControllerConfigName();
            }
            if( (updateFlags & UpdatedConfiguration.LOG_LEVEL) != 0 ) {
                this.RefreshLogLevel();
            }
            if( (updateFlags & UpdatedConfiguration.ORDERED_GAMEPAD_ZONES) != 0 ) {
                this.RefreshConfigZones();
                this.RefreshPhysicalZones();
            }
            if( (updateFlags & UpdatedConfiguration.VISIBLE_GAMEPAD_ZONES) != 0 ) {
                this.RefreshConfigZones();
                this.RefreshPhysicalZones();
            }
        }

        // =======================================================================
        //              Refresh methods
        // =======================================================================

        private void RefreshAll()
        {
            this.RefreshControllerConfigName();
            this.RefreshGamepadConfigs();
            this.RefreshLogLevel();
            this.RefreshGamepadConnected();
            this.RefreshControllerType();
            this.RefreshActivatedContexts();
            this.RefreshActionGroupLabel();
            this.RefreshConfigZones();
            this.RefreshPhysicalZones();
        }

        private void RefreshControllerConfigName()
        {
            this._gamepadConfigName = SteamInputSettings.GetControllerConfigName();
            this.OnGamepadConfigNameChanged.Fire(this._gamepadConfigName);
        }

        /// <summary>
        /// Reload the list of configs available in the Steam config folder, sorted by title.
        /// Public so the config picker's refresh button can trigger a rescan.
        /// </summary>
        private void RefreshGamepadConfigs()
        {
            this._configs.Clear();
            foreach( GamepadConfig config in this._gamepadConfigDaemon.GetConfigs()
                         .OrderBy(c => c.Title, StringComparer.OrdinalIgnoreCase) )
            {
                this._configs.Add(
                    new UIGamepadConfig
                    {
                        Name = config.Name,
                        Title = config.Title,
                        ControllerLabel = config.ControllerType?.GetLabel()
                    }
                );
            }
            this.OnConfigsChanged.Fire(this._configs);
        }

        private void RefreshLogLevel()
        {
            this._logLevel = SteamInputSettings.GetLogLevel();
            OnLogLevelChanged.Fire(this._logLevel);
        }

        private void RefreshGamepadConnected()
        {
            this._gamepadConnected = this._gamepadDaemon.GamepadConnected;
            OnGamepadConnected.Fire(this._gamepadConnected);
        }

        private void RefreshControllerType()
        {
            this._gamepadLabel = this._gamepadConfigDaemon.GetControllerMappings().ControllerType?.GetLabel();
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
            EActionGroup currentActionGroup = this._actionGroupDaemon.GetCurrentActionGroup();
            if( currentActionGroup == EActionGroup.None )
            {
                this._actionGroupLabel = "—";
            }
            else
            {
                VdfAction actionData = this._gamepadConfigDaemon.GetAction(currentActionGroup);
                if( !string.IsNullOrEmpty(actionData.Title) )
                {
                    this._actionGroupLabel = actionData.Title.ToUpperInvariant();
                }
                else
                {
                    this._actionGroupLabel = currentActionGroup.ToString().ToUpperInvariant();
                }
            }
            this.OnActionGroupLabelChanged.Fire(this._actionGroupLabel);
        }

        private string GetLabel(EGamepadZone zone)
        {
            return ModLocalization.GetString("SteamInput_physicalZone_" + zone.Name).ToUpperInvariant();
        }

        private List<EGamepadZone> GetAllZones()
        {
            List<EGamepadZone> gamepadZones = this._gamepadConfigDaemon.GetGamepadZones();
            List<EGamepadZone> orderedZones = SteamInputSettings.GetOrderedGamepadZones();

            // Add all the unknown gamepad zones as hidden zones (at the end of the list)
            foreach( EGamepadZone zone in gamepadZones )
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

            List<EGamepadZone> orderedZones = GetAllZones();
            List<EGamepadZone> visibleZones = SteamInputSettings.GetVisibleGamepadZones();
            for( int i=0; i<orderedZones.Count; i++ )
            {
                EGamepadZone zone = orderedZones[i];
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

        private void RefreshPhysicalZones()
        {
            this._physicalZones.Clear();

            EActionGroup currentActionGroup = this._actionGroupDaemon.GetCurrentActionGroup();

            List<EGamepadZone> orderedZones = GetAllZones();
            List<EGamepadZone> visibleZones = SteamInputSettings.GetVisibleGamepadZones();

            // The base preset, then the layers that superimpose on top of it. A zone is rendered as
            // soon as the base OR any layer defines a section for it (union base ∪ layers). The layer
            // zones are resolved once here, since GetActionLayerZones rescans the preset list.
            Dictionary<EGamepadZone, VdfPresetZone> actionGroupZones = this._gamepadConfigDaemon.GetActionGroupZones(currentActionGroup);
            var layerZones = new List<(string Title, Dictionary<EGamepadZone, VdfPresetZone> Zones)>();
            foreach( VdfLayer layer in this._gamepadConfigDaemon.GetActionLayers(currentActionGroup) )
            {
                layerZones.Add((layer.Title, this._gamepadConfigDaemon.GetActionLayerZones(currentActionGroup, layer.Title)));
            }

            for( int i=0; i<orderedZones.Count; i++ )
            {
                EGamepadZone zone = orderedZones[i];
                if( !visibleZones.Contains(zone) ) {
                    continue;
                }

                UIPhysicalZone z = new UIPhysicalZone
                {
                    Zone = zone,
                    Label = GetLabel(zone),
                };

                // Base preset sections first (no layer title)...
                if( actionGroupZones.TryGetValue(zone, out VdfPresetZone presetZone) )
                {
                    AddSections(z, presetZone, null);
                }
                // ...then the sections each layer adds on top of it.
                foreach( var lz in layerZones )
                {
                    if( lz.Zones.TryGetValue(zone, out VdfPresetZone layerZone) )
                    {
                        AddSections(z, layerZone, lz.Title);
                    }
                }

                // Skip zones neither the base preset nor any layer touches.
                if( z.Sections.Count == 0 )
                {
                    continue;
                }

                this._physicalZones.Add(z);
            }
            OnPhysicalZonesChanged.Fire(this._physicalZones);
        }

        /// <summary>
        /// Append the sections of a preset zone to a physical zone: its normal group first, then one
        /// section per modeshift group. <paramref name="layerTitle"/> is null/empty for the base preset,
        /// or the title of the layer these sections belong to (shown in the section header).
        /// </summary>
        private static void AddSections(UIPhysicalZone zone, VdfPresetZone presetZone, string layerTitle)
        {
            if( presetZone.GroupId != null )
            {
                zone.Sections.Add(
                    new UISection
                    {
                        GroupId = presetZone.GroupId,
                        Modeshift = false,
                        LayerTitle = layerTitle,
                    }
                );
            }
            foreach( string groupId in presetZone.ModeshiftGroupIds )
            {
                zone.Sections.Add(
                    new UISection
                    {
                        GroupId = groupId,
                        Modeshift = true,
                        LayerTitle = layerTitle,
                    }
                );
            }
        }

        // =======================================================================

        public void OpenSettings()
        {
            LOGGER.LogDebug("OpenSettings");
            _settingsDisplayed = true;
            OnShowSettings.Fire(true);
            CloseMenu();
        }

        public void CloseSettings()
        {
            LOGGER.LogDebug("CloseSettings");
            _settingsDisplayed = false;
            OnShowSettings.Fire(false);
        }

        public void OpenMenu()
        {
            LOGGER.LogDebug("OpenMenu");
            _menuDisplayed = true;
            OnShowMenu.Fire(true);
        }

        public void CloseMenu()
        {
            LOGGER.LogDebug("CloseMenu");
            _menuDisplayed = false;
            OnShowMenu.Fire(false);
        }

        public void ToggleMenu()
        {
            if( _menuDisplayed )
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }

        public void MoveZoneUp(UIConfigZone zone)
        {
            List<EGamepadZone> zones = SteamInputSettings.GetOrderedGamepadZones();
            int index = zones.IndexOf(zone.Zone);
            if( index == -1 ) return;
            if( index == 0 ) return;
            
            (zones[index - 1], zones[index]) = (zones[index], zones[index - 1]);
            SteamInputSettings.SetOrderedGamepadZones(zones);
        }

        public void MoveZoneDown(UIConfigZone zone)
        {
            List<EGamepadZone> zones = SteamInputSettings.GetOrderedGamepadZones();
            int index = zones.IndexOf(zone.Zone);
            if( index == -1 ) return;
            if( index == zones.Count - 1 ) return;
            
            (zones[index], zones[index + 1]) = (zones[index + 1], zones[index]);
            SteamInputSettings.SetOrderedGamepadZones(zones);
        }

        public void ToggleZoneVisibility(UIConfigZone zone)
        {
            List<EGamepadZone> visibleZones = SteamInputSettings.GetVisibleGamepadZones();
            if( visibleZones.Contains(zone.Zone) )
            {
                visibleZones.Remove(zone.Zone);
            } else
            {
                visibleZones.Add(zone.Zone);
            }
            SteamInputSettings.SetVisibleGamepadZones(visibleZones);
        }

        public VdfGroup GetGroup(string groupId)
        {
            if (!this._groupsCache.TryGetValue(groupId, out VdfGroup group))
            {
                group = this._gamepadConfigDaemon.GetGroup(groupId);
            }
            return group;
        }

        /// <summary>
        /// True if the group behaves as a mouse (free pointer movement). Such a section shows
        /// a "Mouse — Free movement" banner, on top of any discrete bindings (e.g. a click).
        /// </summary>
        public bool IsMouseGroup(string groupId)
        {
            VdfGroup group = GetGroup(groupId);
            return group != null && MouseModes.Contains(group.Mode);
        }

        /// <summary>
        /// A section is empty when its group is not a mouse group and carries no binding.
        /// Empty sections (normal or modeshift) are hidden; see <see cref="HasNonEmptySection"/>.
        /// </summary>
        public bool IsSectionEmpty(string groupId)
        {
            if( string.IsNullOrEmpty(groupId) )
            {
                return true;
            }
            if( IsMouseGroup(groupId) )
            {
                return false;
            }
            return GetActivators(groupId).Count == 0;
        }

        public bool IsSectionEmpty(UISection section)
        {
            return IsSectionEmpty(section.GroupId);
        }

        /// <summary>True if the zone has at least one non-empty section (normal or modeshift).</summary>
        public bool HasNonEmptySection(UIPhysicalZone zone)
        {
            foreach( UISection section in zone.Sections )
            {
                if( !IsSectionEmpty(section) )
                {
                    return true;
                }
            }
            return false;
        }

        public List<UIActivator> GetActivators(string groupId)
        {
            List<UIActivator> activators = new List<UIActivator>();

            VdfGroup group = GetGroup(groupId);
            if( group == null ) return activators;

            string controllerType = this._gamepadConfigDaemon.GetControllerMappings().ControllerType?.Name;

            foreach( VdfInput vdfInput in group.Inputs )
            {
                EInput input = vdfInput.Name;
                if( input == null ) continue;

                // Show the short/long press chip only when the same input carries several
                // activators (e.g. Menu = short + long); otherwise it is unambiguous.
                bool showPress = vdfInput.Activators.Count > 1;
                string iconText = input.GetLabel(controllerType);

                foreach( VdfActivator vdfActivator in vdfInput.Activators )
                {
                    bool longPress = vdfActivator.Name == EActivator.LongPress;
                    bool modeShift = false;
                    string bindingText = null;
                    List<string> layerTitles = new List<string>();

                    if( vdfActivator.Bindings != null )
                    {
                        foreach( VdfBinding vdfBinding in vdfActivator.Bindings )
                        {
                            if( IsLayerBinding(vdfBinding) )
                            {
                                // A layer activation (controller_action hold_layer N ...) gets its
                                // own highlighted row, like a mode shift but tagged with the layer
                                // name. Drop it when the layer cannot be resolved (unknown position).
                                string layerTitle = GetLayerTitle(vdfBinding);
                                if( !string.IsNullOrEmpty(layerTitle) )
                                {
                                    layerTitles.Add(layerTitle);
                                }
                                continue;
                            }
                            if( vdfBinding.ModeShift )
                            {
                                modeShift = true;
                            }
                            else
                            {
                                bindingText = vdfBinding.Label;
                            }
                        }
                    }

                    // The main row carries the real action and/or the mode shift. It is skipped when
                    // the activator only holds a layer, which would otherwise render as a blank row.
                    if( modeShift || !string.IsNullOrEmpty(bindingText) )
                    {
                        activators.Add(
                            new UIActivator
                            {
                                Input = input,
                                LongPress = longPress,
                                ModeShift = modeShift,
                                BindingText = bindingText,
                                IconText = iconText,
                                PressText = GetPressText(showPress, longPress),
                                ActionText = GetActionText(modeShift, bindingText),
                                Highlighted = modeShift && string.IsNullOrEmpty(bindingText),
                                Note = (modeShift && string.IsNullOrEmpty(bindingText))
                                    ? ModLocalization.GetString("SteamInput_activator_modeshiftHold")
                                    : "",
                            }
                        );
                    }

                    // One highlighted row per layer this activator activates: "Layer (RightClick)".
                    foreach( string layerTitle in layerTitles )
                    {
                        activators.Add(
                            new UIActivator
                            {
                                Input = input,
                                LongPress = longPress,
                                ModeShift = false,
                                BindingText = null,
                                IconText = iconText,
                                PressText = GetPressText(showPress, longPress),
                                ActionText = ModLocalization.GetString("SteamInput_activator_layer"),
                                Highlighted = true,
                                Note = ModLocalization.GetString("SteamInput_sectionLayerSuffix", layerTitle),
                            }
                        );
                    }
                }
            }

            return activators;
        }

        public void RefreshConfigs()
        {
            this._gamepadConfigDaemon.RefreshConfigs();
        }

        // =====================================================
        // Helpers
        // =====================================================

        private static string GetPressText(bool showPress, bool longPress)
        {
            if( !showPress )
            {
                return "";
            }
            return ModLocalization.GetString(longPress
                ? "SteamInput_activator_Long_Press"
                : "SteamInput_activator_Full_Press");
        }

        private static bool IsLayerBinding(VdfBinding binding)
        {
            return binding.EventType == "controller_action"
                && binding.Action != null
                && binding.Action.StartsWith("hold_layer");
        }

        /// <summary>
        /// Title of the layer a <c>hold_layer</c> binding activates, or null when it cannot be
        /// resolved. The action reads "hold_layer N ...", where N is the layer's 1-based position
        /// in the preset list (see <see cref="GamepadConfigDaemon.GetLayerByPosition"/>).
        /// </summary>
        private string GetLayerTitle(VdfBinding binding)
        {
            string[] parts = binding.Action.Split(' ');
            if( parts.Length < 2 || !int.TryParse(parts[1], out int position) )
            {
                return null;
            }
            return this._gamepadConfigDaemon.GetLayerByPosition(position)?.Title;
        }

        private static string GetActionText(bool modeShift, string bindingText)
        {
            // A pure mode shift activator (no real binding) shows the "Modeshift" word;
            // otherwise (incl. a mode shift mixed with a real binding) we show the binding.
            if( modeShift && string.IsNullOrEmpty(bindingText) )
            {
                return ModLocalization.GetString("SteamInput_sectionModeshift");
            }
            return bindingText;
        }
    }
}