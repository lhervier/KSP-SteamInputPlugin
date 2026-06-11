using System;
using UnityEngine;
using TMPro;
using com.github.lhervier.ksp.shared;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.settings
{
    public class LogLevelButtonController : MonoBehaviour
    {
        // Levels in declaration order; the button cycles through them and wraps around.
        private static readonly LogLevel[] Levels = (LogLevel[]) Enum.GetValues(typeof(LogLevel));

        // =======================================
        // Life cycle
        // =======================================

        private CheatSheetViewModel _viewModel;
        public LogLevelButtonController ViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        private TextMeshProUGUI _label;
        public LogLevelButtonController Label(TextMeshProUGUI label)
        {
            this._label = label;
            return this;
        }

        public void Start()
        {
            _viewModel?.OnLogLevelChanged.Add(OnLogLevelChanged);
            if (_viewModel != null)
            {
                OnLogLevelChanged(_viewModel.LogLevel);
            }
        }

        public void OnDestroy()
        {
            _viewModel?.OnLogLevelChanged.Remove(OnLogLevelChanged);
        }

        // ===================================================
        // Methods bound to events
        // ===================================================

        private void OnLogLevelChanged(LogLevel level)
        {
            if (_label != null)
            {
                _label.text = GetLogLevelLabel(level);
            }
        }

        // ======================================
        // Public API
        // ======================================

        /// <summary>Advance to the next level, wrapping back to the first one after the last.</summary>
        public void CycleLogLevel()
        {
            if (_viewModel == null)
            {
                return;
            }
            int index = Array.IndexOf(Levels, _viewModel.LogLevel);
            LogLevel next = Levels[(index + 1) % Levels.Length];
            // Goes through SteamInputGlobalSettings, which fires OnLogLevelChanged back to us.
            _viewModel.LogLevel = next;
        }

        // ======================================
        // Helpers
        // ======================================

        /// <summary>Localized level name, falling back to the enum name if no translation exists.</summary>
        private static string GetLogLevelLabel(LogLevel level)
        {
            string localized = ModLocalization.GetString("SteamInput_logLevel_" + level);
            return string.IsNullOrEmpty(localized) ? level.ToString() : localized;
        }
    }
}
