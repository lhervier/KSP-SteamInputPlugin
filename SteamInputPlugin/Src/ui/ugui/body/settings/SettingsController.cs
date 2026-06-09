using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.shared.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui.button;
using com.github.lhervier.ksp.shared.ugui;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.settings
{
    public class SettingsController : MonoBehaviour
    {
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

        public void Start()
        {
            if( this._backButtonController != null )
            {
                this._backButtonController.OnClick.Add(_viewModel.CloseSettings);
            }
        }

        public void OnDestroy()
        {
            if( this._backButtonController != null )
            {
                this._backButtonController.OnClick.Remove(_viewModel.CloseSettings);
            }
        }
    }
}
