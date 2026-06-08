using com.github.lhervier.ksp.ugui.shared;
using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.ui.styles;
using UnityEngine.Events;
using com.github.lhervier.ksp.ui.ugui.sprites;
using com.github.lhervier.ksp.ui.ugui.titleBar;
using System;
using com.github.lhervier.ksp.ui.ugui.menu;
using com.github.lhervier.ksp.ui.ugui.body;
using System.Collections;
using static com.github.lhervier.ksp.ui.ugui.ModPopupDialogBuilder;

namespace com.github.lhervier.ksp.ui.ugui
{
    public class ModPopupDialogBuilder : IUGUIBuilder<ModPopupDialogController>
    {
        private const string DIALOG_ID = "SteamInputCheatSheetUGUI";
        
        private CheatSheetViewModel _viewModel;
        private OverlayBuilder _overlayBuilder;
        private MenuBuilder _menuBuilder;
        
        private TitleBarBuilder _titleBarBuilder;
        private BodyBuilder _bodyBuilder;
        private PopupBuilder<TitleBarController, BodyController> _popupBuilder;

        public ModPopupDialogBuilder(CheatSheetViewModel viewModel)
        {
            Init(viewModel);
            this._popupBuilder = new PopupBuilder<TitleBarController, BodyController>(
                DIALOG_ID,
                _titleBarBuilder,
                _bodyBuilder
            );
        }

        public ModPopupDialogBuilder(
            CheatSheetViewModel viewModel,
            Vector2 initialPosition
        )
        {
            Init(viewModel);
            this._popupBuilder = new PopupBuilder<TitleBarController, BodyController>(
                initialPosition,
                DIALOG_ID,
                _titleBarBuilder,
                _bodyBuilder
            );
        }

        private void Init(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            
            this._titleBarBuilder = new TitleBarBuilder(viewModel);
            this._bodyBuilder = new BodyBuilder(viewModel);

            this._overlayBuilder = new OverlayBuilder(viewModel);
            this._menuBuilder = new MenuBuilder(viewModel);
        }

        /// <summary>
        /// Spawn the cheat-sheet popup window and return its controller, or null if KSP failed to spawn
        /// it. The caller drives the window through the returned controller.
        /// </summary>
        public ModPopupDialogController Create()
        {
            PopupController dialogController = _popupBuilder.Create();
            if( dialogController == null ) return null;

            ModPopupDialogController controller = dialogController.GetGameObject().AddComponent<ModPopupDialogController>();
            controller.Initialize(_viewModel);
            controller.BindPopupController(dialogController);
            controller.BindOverlayBuilder(_overlayBuilder);
            controller.BindMenuBuilder(_menuBuilder);
            
            return controller;
        }

        // ==============================================================
        // Controller
        // ==============================================================

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
            public void BindPopupController(PopupController controller)
            {
                this._popupController = controller;
            }

            /// <summary>Inject the overlay builder.</summary>
            public void BindOverlayBuilder(OverlayBuilder builder)
            {
                this._overlayBuilder = builder;
            }

            /// <summary>Inject the menu builder.</summary>
            public void BindMenuBuilder(MenuBuilder builder)
            {
                this._menuBuilder = builder;
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
                    _overlayController = _overlayBuilder.Create(() => ViewModel.CloseMenu());
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
}
