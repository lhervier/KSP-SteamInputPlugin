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
    public class ModPopupBuilder : IUGUIBuilder<ModPopupDialogController>
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

        public ModPopupBuilder(CheatSheetViewModel viewModel)
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

        public ModPopupBuilder Position(Vector2 position)
        {
            this._position = position;
            this._hasPosition = true;
            return this;
        }

        public ModPopupBuilder DeletePosition()
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
                .ContentBuilder(_bodyBuilder)
                .Size(new Vector2(SteamInputPalette.WindowWidth, SteamInputPalette.WindowHeight));
            if( _hasPosition )
            {
                builder = builder.Position(this._position);
            }
            PopupController dialogController = builder.Build();
            if( dialogController == null ) return null;

            return dialogController
                .GetGameObject()
                .AddComponent<ModPopupDialogController>()
                .ViewModel(_viewModel)
                .PopupController(dialogController)
                .OverlayBuilder(_overlayBuilder)
                .MenuBuilder(_menuBuilder)
                .Build();
        }
    }
}
