using UnityEngine;

namespace com.github.lhervier.ksp.steaminput.ui.ugui
{
    public abstract class BaseSteamInputController : MonoBehaviour
    {
        protected CheatSheetViewModel ViewModel => _viewModel;
        private CheatSheetViewModel _viewModel;

        public void Initialize(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }
    }
}