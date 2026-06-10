using UnityEngine;
using com.github.lhervier.ksp.steaminput.ui.ugui.menu;
using com.github.lhervier.ksp.shared.ugui.popup;
using com.github.lhervier.ksp.shared.ugui.overlay;

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
        public ModPopupController PopupController(PopupController controller)
        {
            this._popupController = controller;
            return this;
        }

        private OverlayController _overlayController = null;
        public ModPopupController OverlayController(OverlayController overlayController)
        {
            this._overlayController = overlayController;
            return this;
        }

        private MenuBuilder.MenuController _menuController = null;
        public ModPopupController MenuController(MenuBuilder.MenuController menuController)
        {
            this._menuController = menuController;
            return this;
        }

        /// <summary>
        /// Unity callback. Sets up the controller; its counterpart is <see cref="OnDestroy"/>.
        /// </summary>
        public void Start()
        {
            if( _overlayController != null )
            {
                _overlayController.OnClose.Add(ViewModel.CloseMenu);
            }
            if( ViewModel != null )
            {
                ViewModel.OnShowMenu.Add(OnShowMenu);
                OnShowMenu(ViewModel.MenuDisplayed);
            }
        }

        /// <summary>
        /// Unity callback. Tears down what <see cref="Start"/> set up.
        /// </summary>
        public void OnDestroy()
        {
            if( ViewModel != null )
            {
                ViewModel.OnShowMenu.Remove(OnShowMenu);
            }
            if( _overlayController != null )
            {
                _overlayController.OnClose.Remove(ViewModel.CloseMenu);
            }
        }

        // =====================================
        // Methods bound to events
        // =====================================

        /// <summary>Show or hide the menu and its overlay.</summary>
        private void OnShowMenu(bool show)
        {
            _overlayController.gameObject.SetActive(show);
            _menuController.gameObject.SetActive(show);
        }
        
        // =====================
        // Public API
        // =====================

        public void Show() => _popupController.Show();
        public void Hide() => _popupController.Hide();
        public void Dismiss() => _popupController.Dismiss();
    }
}
