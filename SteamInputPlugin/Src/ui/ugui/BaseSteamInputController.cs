using UnityEngine;

namespace com.github.lhervier.ksp.steaminput.ui.ugui
{
    public abstract class BaseSteamInputController : MonoBehaviour
    {
        protected CheatSheetViewModel ViewModel => _viewModel;
        private CheatSheetViewModel _viewModel;

        internal void SetViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }
    }

    public static class SteamInputControllerExtensions
    {
        /// <summary>
        /// Extension method to avoid fluent builder inheritance problem
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public static T BindViewModel<T>(this T controller, CheatSheetViewModel vm)
            where T : BaseSteamInputController
        {
            controller.SetViewModel(vm);
            return controller;
        }
    }

}