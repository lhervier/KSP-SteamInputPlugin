using UnityEngine;
using com.github.lhervier.ksp.steaminput.ui.ugui.titleBar;
using com.github.lhervier.ksp.steaminput.ui.ugui.menu;
using com.github.lhervier.ksp.steaminput.ui.ugui.body;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.popup;
using static com.github.lhervier.ksp.steaminput.ui.ugui.ModPopupDialogBuilder;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;

namespace com.github.lhervier.ksp.steaminput.ui.ugui
{
    public class ModPopupDialogBuilder : IUGUIBuilder<ModPopupDialogController>
    {
        private const string DIALOG_ID = "SteamInputCheatSheetUGUI";
        
        private CheatSheetViewModel _viewModel;
        private OverlayBuilder _overlayBuilder;
        private MenuBuilder _menuBuilder;
        
        private Vector2 _position;
        private bool _hasPosition = false;
        private TitleBarBuilder _titleBarBuilder;
        private BodyBuilder _bodyBuilder;
        private PopupBuilder<TitleBarController, BodyController> _popupBuilder;

        public ModPopupDialogBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            
            this._titleBarBuilder = new TitleBarBuilder(viewModel);
            this._bodyBuilder = new BodyBuilder(viewModel);

            this._overlayBuilder = new OverlayBuilder(viewModel);
            this._menuBuilder = new MenuBuilder(viewModel);

            this._popupBuilder = new PopupBuilder<TitleBarController, BodyController>();
        }

        // =============================================
        // Build parameters
        // =============================================

        public ModPopupDialogBuilder Position(Vector2 position)
        {
            this._position = position;
            this._hasPosition = true;
            return this;
        }

        public ModPopupDialogBuilder DeletePosition()
        {
            this._hasPosition = false;
            return this;
        }

        // =============================================
        // Builder
        // =============================================

        /// <summary>
        /// Spawn the cheat-sheet popup window and return its controller, or null if KSP failed to spawn
        /// it. The caller drives the window through the returned controller.
        /// </summary>
        public ModPopupDialogController Build()
        {
            var builder = _popupBuilder
                .PopupID(DIALOG_ID)
                .Title(ModLocalization.GetString("SteamInput_titleHelp"))
                .Icon(SpritesTitleBar.GamepadIconSprite)
                .TitleBarBuilder(_titleBarBuilder)
                .ContentBuilder(_bodyBuilder);
            if( _hasPosition )
            {
                builder = builder.Position(this._position);
            }
            PopupController dialogController = builder.Build();
            if( dialogController == null ) return null;

            return dialogController
                .GetGameObject()
                .AddComponent<ModPopupDialogController>()
                .BindViewModel(_viewModel)
                .BindPopupController(dialogController)
                .BindOverlayBuilder(_overlayBuilder)
                .BindMenuBuilder(_menuBuilder);
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
            public ModPopupDialogController BindPopupController(PopupController controller)
            {
                this._popupController = controller;
                return this;
            }

            /// <summary>Inject the overlay builder.</summary>
            public ModPopupDialogController BindOverlayBuilder(OverlayBuilder builder)
            {
                this._overlayBuilder = builder;
                return this;
            }

            /// <summary>Inject the menu builder.</summary>
            public ModPopupDialogController BindMenuBuilder(MenuBuilder builder)
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
