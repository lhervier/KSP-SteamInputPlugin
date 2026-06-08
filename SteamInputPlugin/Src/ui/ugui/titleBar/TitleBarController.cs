using com.github.lhervier.ksp.shared.ugui.button;
using com.github.lhervier.ksp.shared.ugui.popup;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.titleBar
{
    public class TitleBarController : PopupTitleBarController
    {
        private ButtonController _menuButtonController;
        private CheatSheetViewModel _viewModel;

        public void BindViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public void BindMenuButtonController(ButtonController menuButtonController)
        {
            this._menuButtonController = menuButtonController;
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