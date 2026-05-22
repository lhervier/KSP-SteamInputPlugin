using System;
using com.github.lhervier.ksp.ui.styles;
using UnityEngine;
using UnityEngine.Events;

namespace com.github.lhervier.ksp.ui.ugui
{
    /// <summary>
    /// uGUI test shell via KSP PopupDialog (same approach as Trajectories MainGUI).
    /// </summary>
    internal sealed class CheatSheetUGUIWindow
    {
        private PopupDialogBuilder _popupDialogBuilder;
        private PopupDialog _popupDialog = null;
        private CheatSheetViewModel _viewModel;

        public void Initialize(CheatSheetViewModel viewModel)
        {
            _viewModel = viewModel;
            this._popupDialogBuilder = new PopupDialogBuilder(viewModel);
        }

        // ===============================================================
        // Public API
        // ===============================================================

        public void Show()
        {
            if (_popupDialog == null)
            {
                _popupDialog = this._popupDialogBuilder.CreatePopupDialog();
                _popupDialog?.onDestroy.AddListener(OnPopupDestroyed);
            }

            _popupDialog?.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _popupDialog?.gameObject.SetActive(false);
        }

        public void Destroy()
        {
            _popupDialog?.onDestroy.RemoveListener(OnPopupDestroyed);
            _popupDialog?.Dismiss();
            _popupDialog = null;
        }

        public void OnPopupDestroyed()
        {
            _popupDialog = null;
        }
    }
}
