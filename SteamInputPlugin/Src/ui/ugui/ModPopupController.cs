using UnityEngine;
using com.github.lhervier.ksp.steaminput.ui.ugui.titleBar;
using com.github.lhervier.ksp.steaminput.ui.ugui.menu;
using com.github.lhervier.ksp.steaminput.ui.ugui.body;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.popup;
using static com.github.lhervier.ksp.steaminput.ui.ugui.ModPopupBuilder;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui.styles;
using com.github.lhervier.ksp.steaminput.ui.styles;

namespace com.github.lhervier.ksp.steaminput.ui.ugui
{
    public class ModPopupDialogController : BaseSteamInputController
    {
        private OverlayBuilder _overlayBuilder;
        private MenuBuilder _menuBuilder;

        private OverlayBuilder.OverlayController _overlayController = null;
        private MenuBuilder.MenuController _menuController = null;
        private PopupController _popupController = null;
        
        public EventVoid OnClosed => _popupController.OnClosed;
        public EventData<Vector2> OnPositionCaptured => _popupController.OnPositionCaptured;
        
        // =========================
        // Life cycle
        // =========================

        // Dependencies injected by the builder right after AddComponent, before Start() runs.

        /// <summary>Inject the popup controller.</summary>
        public ModPopupDialogController PopupController(PopupController controller)
        {
            this._popupController = controller;
            return this;
        }

        /// <summary>Inject the overlay builder.</summary>
        public ModPopupDialogController OverlayBuilder(OverlayBuilder builder)
        {
            this._overlayBuilder = builder;
            return this;
        }

        /// <summary>Inject the menu builder.</summary>
        public ModPopupDialogController MenuBuilder(MenuBuilder builder)
        {
            this._menuBuilder = builder;
            return this;
        }

        /// <summary>
        /// Unity callback. Sets up the controller; its counterpart is <see cref="OnDestroy"/>.
        /// </summary>
        public void Start()
        {
            ViewModel?.OnShowMenu.Add(OnShowMenu);
            if( ViewModel != null )
            {
                OnShowMenu(ViewModel.MenuDisplayed);
            }
        }

        /// <summary>
        /// Unity callback. Tears down what <see cref="Start"/> set up.
        /// </summary>
        public void OnDestroy()
        {
            ViewModel?.OnShowMenu.Remove(OnShowMenu);
        }

        /// <summary>Show or hide the menu and its overlay.</summary>
        private void OnShowMenu(bool show)
        {
            if( _overlayController == null )
            {
                _overlayController = _overlayBuilder.Build(() => ViewModel.CloseMenu());
                _overlayController.transform.SetParent(gameObject.transform, false);
            }

            if( _menuController == null )
            {
                _menuController = _menuBuilder.Create();
                _menuController.transform.SetParent(gameObject.transform, false);
            }

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
