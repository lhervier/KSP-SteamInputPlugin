using UnityEngine;
using com.github.lhervier.ksp.shared.ugui.button;
using com.github.lhervier.ksp.shared.ugui.combo;
using com.github.lhervier.ksp.shared;
using System;
using System.Collections.Generic;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.settings
{
    public class SettingsController : MonoBehaviour
    {
        private readonly List<string> _levels = new List<string>();
        
        // ===================================
        // Life cycle
        // ===================================

        private CheatSheetViewModel _viewModel;
        public SettingsController ViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        private ButtonController _backButtonController;
        public SettingsController BackButtonController(ButtonController backButtonController)
        {
            this._backButtonController = backButtonController;
            return this;
        }

        private ComboController _logLevelComboController;
        public SettingsController LogLevelComboController(ComboController logLevelComboController)
        {
            this._logLevelComboController = logLevelComboController;
            return this;
        }

        public void Awake()
        {
            LogLevel[] levels = (LogLevel[]) Enum.GetValues(typeof(LogLevel));
            foreach( LogLevel level in levels )
            {
                _levels.Add(level.ToString());
            }
        }

        public void Start()
        {
            if( this._backButtonController != null )
            {
                this._backButtonController.OnClick.Add(_viewModel.CloseSettings);
            }
            if( _logLevelComboController != null )
            {
                _logLevelComboController.SetOptions(
                    _levels,
                    _viewModel.LogLevel.ToString()
                );
                _logLevelComboController.OnSelect.Add(OnSelect);
            }
            if( _viewModel != null )
            {
                _viewModel.OnLogLevelChanged.Add(OnLogLevelChanged);
            }
        }

        public void OnDestroy()
        {
            if( _viewModel != null )
            {
                _viewModel.OnLogLevelChanged.Remove(OnLogLevelChanged);
            }
            if( _logLevelComboController != null )
            {
                _logLevelComboController.OnSelect.Remove(OnSelect);
            }
            if( this._backButtonController != null )
            {
                this._backButtonController.OnClick.Remove(_viewModel.CloseSettings);
            }
        }

        // =====================================
        // Methods bound to events
        // =====================================

        private void OnLogLevelChanged(LogLevel logLevel)
        {
            if( _logLevelComboController != null )
            {
                _logLevelComboController.Select(logLevel.ToString());
            }
        }

        private void OnSelect(string value)
        {
            if( Enum.TryParse(value, out LogLevel logLevel) )
            {
                _viewModel.LogLevel = logLevel;
            }
        }
    }
}
