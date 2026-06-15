using com.github.lhervier.ksp.shared.ugui.button;
using UnityEngine;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.titleBar
{
    public class TitleBarController : MonoBehaviour
    {
        // ================================================
        // Life cycle
        // ================================================

        private CheatSheetViewModel _viewModel;
        public TitleBarController WithViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        private ButtonController _menuButtonController;
        public TitleBarController WithMenuButtonController(ButtonController menuButtonController)
        {
            this._menuButtonController = menuButtonController;
            return this;
        }

        public void Start()
        {
            if( this._menuButtonController != null && _viewModel != null )
            {
                this._menuButtonController.OnClick.Add(_viewModel.ToggleMenu);
            }
        }

        public void OnDestroy()
        {
            if( this._menuButtonController != null && _viewModel != null )
            {
                this._menuButtonController.OnClick.Remove(_viewModel.ToggleMenu);
            }
        }
    }
}