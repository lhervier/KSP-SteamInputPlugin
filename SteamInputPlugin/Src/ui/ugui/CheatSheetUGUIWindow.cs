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
                _popupDialogController.OnClosed.Add(_OnClosed);
                _popupDialogController.OnPositionCaptured.Add(_OnPositionCaptured);
            }
            _popupDialogController.Show();
        }

        private void _OnClosed()
        {
            this.OnClosed.Fire();
        }

        private void _OnPositionCaptured(Vector2 position)
        {
            this.OnPositionCaptured.Fire(position);
        }

        public void SetPosition(Vector2 position)
        {
            if (_popupDialogController == null ) return;
            _popupDialogController.SetPosition(position);
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
            if (_popupDialogController == null) return;
            _popupDialogController.Hide();
        }

        public void Dismiss()
        {
            if (_popupDialogController == null) return;
            _popupDialogController.OnClosed.Remove(_OnClosed);
            _popupDialogController.OnPositionCaptured.Remove(_OnPositionCaptured);
            _popupDialogController.Dismiss();
            _popupDialogController = null;
        }
    }
}
