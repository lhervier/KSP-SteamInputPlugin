using UnityEngine;
using com.github.lhervier.ksp.steaminput.ui.ugui.body.zones;
using com.github.lhervier.ksp.steaminput.ui.ugui.body.selector;
using com.github.lhervier.ksp.steaminput.ui.ugui.body.settings;
using System;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body
{
    public class BodyController : MonoBehaviour
    {
        private CheatSheetViewModel _viewModel;
        private GameObject _content;
        private SelectorBuilder _selectorBuilder;
        private ZoneListBuilder _zoneListBuilder;
        private SettingsBuilder _settingsBuilder;

        private SelectorBuilder.SelectorController _selectorController;
        private ZoneListBuilder.ZoneListController _zoneListController;
        private SettingsBuilder.SettingsController _settingsController;

        public void BindViewModel(CheatSheetViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public void BindContent(GameObject content)
        {
            this._content = content;
        }

        public void BindSelectorBuilder(SelectorBuilder builder)
        {
            this._selectorBuilder = builder;
        }

        public void BindZoneListBuilder(ZoneListBuilder builder)
        {
            this._zoneListBuilder = builder;
        }

        public void BindSettingsBuilder(SettingsBuilder builder)
        {
            this._settingsBuilder = builder;
        }

        public void Start()
        {
            _viewModel?.OnShowSettings.Add(OnShowSettings);
            if( _viewModel != null )
            {
                OnShowSettings(_viewModel.SettingsDisplayed);
            }
        }

        public void OnDestroy()
        {
            _viewModel?.OnShowSettings.Remove(OnShowSettings);
        }

        private void OnShowSettings(bool show)
        {
            if( _selectorBuilder == null || _zoneListBuilder == null || _settingsBuilder == null )
            {
                throw new ArgumentException("Builders not binded");
            }

            if( _selectorController == null )
            {
                _selectorController = _selectorBuilder.Create();
                _selectorController.transform.SetParent(_content.transform, false);
            }

            if( _zoneListController == null )
            {
                _zoneListController = _zoneListBuilder.Create();
                _zoneListController.transform.SetParent(_content.transform, false);
            }

            if( _settingsController == null )
            {
                _settingsController = _settingsBuilder.Create();
                _settingsController.transform.SetParent(_content.transform, false);
            }

            _settingsController.gameObject.SetActive(show);
            _zoneListController.gameObject.SetActive(!show);
            _selectorController.gameObject.SetActive(!show);
        }
    }
}
