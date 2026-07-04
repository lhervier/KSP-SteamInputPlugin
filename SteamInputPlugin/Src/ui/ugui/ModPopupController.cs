using UnityEngine;
using com.github.lhervier.ksp.shared.ugui.popup;

namespace com.github.lhervier.ksp.steaminput.ui.ugui
{
    public class ModPopupController : BaseSteamInputController
    {
        public EventVoid OnClosed => _popupController.OnClosed;
        public EventData<Vector2> OnPositionCaptured => _popupController.OnPositionCaptured;

        // =========================
        // Life cycle
        // =========================

        // Dependencies injected by the builder right after AddComponent, before Start() runs.

        private PopupController _popupController = null;
        public ModPopupController WithPopupController(PopupController controller)
        {
            this._popupController = controller;
            return this;
        }

        // =====================
        // Public API
        // =====================

        public void Show() => _popupController.Show();
        public void Hide() => _popupController.Hide();
        public void Dismiss() => _popupController.Dismiss();
    }
}
