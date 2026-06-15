using System.Collections.Generic;
using com.github.lhervier.ksp.steaminput.ui.model;
using com.github.lhervier.ksp.shared;
using com.github.lhervier.ksp.shared.ugui.button;
using com.github.lhervier.ksp.shared.ugui.combo;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.selector
{
    /// <summary>
    /// Adapter between the config ViewModel and a shared ComboController: maps the available configs
    /// (plus a "none" entry) into combo options, mirrors the current selection, and pushes the user's
    /// choice back to the ViewModel. Also owns the refresh button that rescans the config folder.
    /// </summary>
    public class SelectorController : BaseSteamInputController
    {
        private List<UIGamepadConfig> _configs = new List<UIGamepadConfig>();

        private ComboController _combo;
        public SelectorController WithComboController(ComboController combo)
        {
            this._combo = combo;
            return this;
        }

        private ButtonController _refreshButtonController;
        public SelectorController WithRefreshButtonController(ButtonController refreshButton)
        {
            this._refreshButtonController = refreshButton;
            return this;
        }

        public void Start()
        {
            if (_combo != null)
            {
                _combo.OnSelect.Add(OnComboSelect);
            }
            if (_refreshButtonController != null)
            {
                _refreshButtonController.OnClick.Add(ViewModel.RefreshConfigs);
            }

            this.ViewModel.OnConfigsChanged.Add(OnConfigsChanged);
            this.ViewModel.OnGamepadConfigNameChanged.Add(OnConfigNameChanged);
            OnConfigsChanged(this.ViewModel.Configs);
        }

        public void OnDestroy()
        {
            if (_combo != null)
            {
                _combo.OnSelect.Remove(OnComboSelect);
            }
            if (_refreshButtonController != null)
            {
                _refreshButtonController.OnClick.Remove(ViewModel.RefreshConfigs);
            }
            this.ViewModel?.OnConfigsChanged.Remove(OnConfigsChanged);
            this.ViewModel?.OnGamepadConfigNameChanged.Remove(OnConfigNameChanged);
        }

        // ====================================
        // Combo wiring (passed to the combo at build time)
        // ====================================

        /// <summary>
        /// Resolves a config name to its config, or null for the "none" entry / an unknown name.
        /// Given to the item content builder as a LIVE lookup: it reads the current list on each call.
        /// </summary>
        public UIGamepadConfig ResolveConfig(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            foreach (UIGamepadConfig config in _configs)
            {
                if (config.Name == name) return config;
            }
            return null;
        }

        /// <summary>
        /// Header / current-value text: the selected config's title, the "none" placeholder for an
        /// empty name, or the raw name when the selected config is not in the current list.
        /// </summary>
        public string LabelFor(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return ModLocalization.GetString("SteamInput_configNone");
            }
            UIGamepadConfig config = ResolveConfig(name);
            return config != null ? config.Title : name;
        }

        // ========================================
        // Methods bound to events
        // ========================================

        private void OnConfigsChanged(List<UIGamepadConfig> configs)
        {
            _configs = configs ?? new List<UIGamepadConfig>();
            if (_combo == null) return;

            // "none" first (empty id), then one option per config (the raw config name is the id).
            var options = new List<string> { "" };
            foreach (UIGamepadConfig config in _configs)
            {
                options.Add(config.Name);
            }
            _combo.SetOptions(options, this.ViewModel.GamepadConfigName);
        }

        private void OnConfigNameChanged(string name)
        {
            if (_combo != null) _combo.Select(name);
        }

        private void OnComboSelect(string name)
        {
            this.ViewModel.GamepadConfigName = name;
        }
    }
}
