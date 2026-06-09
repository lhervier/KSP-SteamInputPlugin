using com.github.lhervier.ksp.shared.ugui.scrollableview;
using com.github.lhervier.ksp.steaminput.ui.ugui.body.cheatsheet;
using com.github.lhervier.ksp.steaminput.ui.ugui.body.settings;
using UnityEngine;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body
{
    /// <summary>
    /// Drives the body's two views: shows the settings screen or the scrollable cheat sheet depending on
    /// the view model's settings toggle. Both views are built up-front and fill the body; only one is
    /// active at a time.
    /// </summary>
    public class BodyController : MonoBehaviour
    {
        private CheatSheetViewModel _viewModel;
        private ScrollableViewController _cheatSheet;
        private SettingsBuilder.SettingsController _settings;

        // ==================================
        // Life cycle
        // ==================================

        // Dependencies injected by the builder right after AddComponent, before Start() runs.

        public BodyController ViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        public BodyController CheatSheetController(ScrollableViewController cheatSheet)
        {
            this._cheatSheet = cheatSheet;
            return this;
        }

        public BodyController SettingsController(SettingsBuilder.SettingsController settings)
        {
            this._settings = settings;
            return this;
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

        // =========================================
        // Methods bound to events
        // =========================================

        // Show the settings screen or the cheat sheet, never both.
        private void OnShowSettings(bool show)
        {
            _settings?.gameObject.SetActive(show);
            _cheatSheet?.gameObject.SetActive(!show);
        }
    }
}
