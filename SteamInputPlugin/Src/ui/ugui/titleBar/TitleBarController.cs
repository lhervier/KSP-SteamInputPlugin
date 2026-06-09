using com.github.lhervier.ksp.shared.ugui.button;
using UnityEngine;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.titleBar
{
    public class TitleBarController : MonoBehaviour
    {
        private ButtonController _menuButtonController;
        private CheatSheetViewModel _viewModel;

        // ================================================
        // Life cycle
        // ================================================

        public TitleBarController ViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        public TitleBarController MenuButtonController(ButtonController menuButtonController)
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