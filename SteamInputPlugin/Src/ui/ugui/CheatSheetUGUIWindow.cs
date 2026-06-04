using System;
using com.github.lhervier.ksp.ui.styles;
using UnityEngine;
using UnityEngine.Events;

namespace com.github.lhervier.ksp.ui.ugui
{
    /// <summary>
    /// uGUI test shell via KSP PopupDialog (same approach as Trajectories MainGUI).
    /// </summary>
    internal sealed class CheatSheetUGUIWindow : MonoBehaviour
    {
        private PopupDialogBuilder _popupDialogBuilder;
        private PopupDialogBuilder.PopupDialogController _popupDialogController = null;
        public EventVoid OnClosed = new EventVoid("SteamInput.CheatSheetUGUIWindow.OnClosed");
        public EventData<Vector2> OnPositionCaptured = new EventData<Vector2>("SteamInput.CheatSheetUGUIWindow.OnMoved");

        // ===============================================================
        // Life Cycle
        // ===============================================================
        
        public void Initialize(CheatSheetViewModel viewModel)
        {
            this._popupDialogBuilder = new PopupDialogBuilder(viewModel);
        }

        public void Start()
        {
            // Our window persists across scenes. KSP fails to restore its interactivity after a
            // scene change if a modal dialog was up beforehand (see RestoreInteractivity), so we
            // re-assert it once the new scene has loaded.
            GameEvents.onLevelWasLoaded.Add(OnLevelWasLoaded);
        }

        public void OnDestroy()
        {
            GameEvents.onLevelWasLoaded.Remove(OnLevelWasLoaded);
        }

        /// <summary>
        /// Re-assert the window interactivity after a scene change (no-op if it is not open),
        /// to work around KSP leaving a surviving non-modal dialog with blocksRaycasts stuck false.
        /// </summary>
        private void OnLevelWasLoaded(GameScenes scene)
        {
            this.RestoreInteractivity();
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

        public void Dismiss()
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
