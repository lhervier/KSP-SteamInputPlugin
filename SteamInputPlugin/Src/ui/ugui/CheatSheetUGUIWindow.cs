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
        public EventVoid OnClosed = new EventVoid("SteamInput.CheatSheetUGUIWindow.OnClosed");
        public EventData<Vector2> OnPositionCaptured = new EventData<Vector2>("SteamInput.CheatSheetUGUIWindow.OnMoved");

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

        public void SetPosition(Vector2 position)
        {
            if (_popupDialog == null ) return;
            if( _popupDialog.RTrf == null ) return;
            Vector3 lp = _popupDialog.RTrf.localPosition;
            _popupDialog.RTrf.localPosition = new Vector3(position.x, position.y, lp.z);

        }

        public void Hide()
        {
            CaptureWindowPosition();
            _popupDialog?.gameObject.SetActive(false);
        }

        public void Destroy()
        {
            CaptureWindowPosition();
            _popupDialog?.onDestroy.RemoveListener(OnPopupDestroyed);
            _popupDialog?.Dismiss();
            _popupDialog = null;
        }

        public void OnPopupDestroyed()
        {
            // Dismissed by KSP (e.g. Escape) without going through Hide(): grab the position first,
            // then let the owner resync (toolbar toggle, other windows).
            CaptureWindowPosition();
            _popupDialog = null;
            OnClosed.Fire();
        }

        // ===============================================================

        /// <summary>
        /// Remember where the user dragged the window (the draggable handler's localPosition),
        /// so it can be restored when the window is recreated or the game restarts.
        /// </summary>
        private void CaptureWindowPosition()
        {
            if (_popupDialog != null && _popupDialog.RTrf != null)
            {
                OnPositionCaptured.Fire(_popupDialog.RTrf.localPosition);
            }
        }
    }
}
