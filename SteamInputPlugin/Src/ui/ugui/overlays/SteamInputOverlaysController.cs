using UnityEngine;
using com.github.lhervier.ksp.steaminput.ui.ugui.menu;
using com.github.lhervier.ksp.shared.ugui.overlay;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.overlays
{
    /// <summary>
    /// Drives the popup overlays: the drop-down menu and the full-window click trap behind it. Both are
    /// shown and hidden together, following the ViewModel's menu-visibility state, and a click on the
    /// trap closes the menu. Self-contained so the popup only has to graft the overlay root.
    /// </summary>
    public class SteamInputOverlaysController : MonoBehaviour
    {
        private CheatSheetViewModel _viewModel;
        public SteamInputOverlaysController WithViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        private OverlayController _overlayController;
        public SteamInputOverlaysController WithOverlayController(OverlayController overlayController)
        {
            this._overlayController = overlayController;
            return this;
        }

        private MenuBuilder.MenuController _menuController;
        public SteamInputOverlaysController WithMenuController(MenuBuilder.MenuController menuController)
        {
            this._menuController = menuController;
            return this;
        }

        /// <summary>
        /// Unity callback. Wires the overlays to the ViewModel; its counterpart is <see cref="OnDestroy"/>.
        /// </summary>
        public void Start()
        {
            if( _viewModel != null )
            {
                if( _overlayController != null )
                {
                    _overlayController.OnClose.Add(_viewModel.CloseMenu);
                }
                _viewModel.OnShowMenu.Add(OnShowMenu);
                OnShowMenu(_viewModel.MenuDisplayed);
            }
        }

        /// <summary>
        /// Unity callback. Tears down what <see cref="Start"/> wired.
        /// </summary>
        public void OnDestroy()
        {
            if( _viewModel != null )
            {
                _viewModel.OnShowMenu.Remove(OnShowMenu);
                if( _overlayController != null )
                {
                    _overlayController.OnClose.Remove(_viewModel.CloseMenu);
                }
            }
        }

        // Show or hide the menu and the click trap together.
        private void OnShowMenu(bool show)
        {
            if( _overlayController != null )
            {
                _overlayController.gameObject.SetActive(show);
            }
            if( _menuController != null )
            {
                _menuController.gameObject.SetActive(show);
            }
        }
    }
}
