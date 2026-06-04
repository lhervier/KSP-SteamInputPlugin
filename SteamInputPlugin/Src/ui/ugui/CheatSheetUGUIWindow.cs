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
        private PopupDialogBuilder.PopupDialogController _popupDialogController = null;
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
            if (_popupDialogController == null)
            {
                _popupDialogController = this._popupDialogBuilder.Create();
                if (_popupDialogController == null) return;    // Spawn failed
                _popupDialogController.GetPopupDialog()?.onDestroy.AddListener(OnPopupDestroyed);
            }

            _popupDialogController.GetPopupDialog()?.gameObject.SetActive(true);
        }

        public void SetPosition(Vector2 position)
        {
            if (_popupDialogController == null ) return;
            PopupDialog popupDialog = _popupDialogController.GetPopupDialog();
            if( popupDialog.RTrf == null ) return;
            Vector3 lp = popupDialog.RTrf.localPosition;
            popupDialog.RTrf.localPosition = new Vector3(position.x, position.y, lp.z);

        }

        /// <summary>
        /// Make the window visible once it has been positioned (see <see cref="PopupDialogBuilder.PopupDialogController.Reveal"/>).
        /// </summary>
        public void Reveal()
        {
            if (_popupDialogController == null) return;
            _popupDialogController.Reveal();
        }

        /// <summary>
        /// Re-enable pointer interaction on the window (see <see cref="PopupDialogBuilder.PopupDialogController.RestoreInteractivity"/>).
        /// </summary>
        public void RestoreInteractivity()
        {
            if (_popupDialogController == null) return;
            _popupDialogController.RestoreInteractivity();
        }

        public void Hide()
        {
            CaptureWindowPosition();
            if (_popupDialogController == null) return;
            _popupDialogController.GetPopupDialog()?.gameObject.SetActive(false);
        }

        public void Destroy()
        {
            CaptureWindowPosition();
            if (_popupDialogController == null) return;
            PopupDialog popupDialog = _popupDialogController.GetPopupDialog();
            popupDialog?.onDestroy.RemoveListener(OnPopupDestroyed);
            popupDialog?.Dismiss();
            _popupDialogController = null;
        }

        public void OnPopupDestroyed()
        {
            // Dismissed by KSP (e.g. Escape) without going through Hide(): grab the position first,
            // then let the owner resync (toolbar toggle, other windows).
            CaptureWindowPosition();
            _popupDialogController = null;
            OnClosed.Fire();
        }

        // ===============================================================

        /// <summary>
        /// Remember where the user dragged the window (the draggable handler's localPosition),
        /// so it can be restored when the window is recreated or the game restarts.
        /// </summary>
        private void CaptureWindowPosition()
        {
            if (_popupDialogController == null) return;
            PopupDialog popupDialog = _popupDialogController.GetPopupDialog();
            if (popupDialog != null && popupDialog.RTrf != null)
            {
                OnPositionCaptured.Fire(popupDialog.RTrf.localPosition);
            }
        }
    }
}
