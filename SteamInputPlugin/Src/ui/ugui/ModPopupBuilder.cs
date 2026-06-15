using UnityEngine;
using com.github.lhervier.ksp.steaminput.ui.ugui.titleBar;
using com.github.lhervier.ksp.steaminput.ui.ugui.menu;
using com.github.lhervier.ksp.steaminput.ui.ugui.body;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.popup;
using com.github.lhervier.ksp.shared.ugui.overlay;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.shared;

namespace com.github.lhervier.ksp.steaminput.ui.ugui
{
    public class ModPopupBuilder : IUGUIBuilder<ModPopupController>
    {
        private const string DIALOG_ID = "SteamInputCheatSheetUGUI";
        
        // =============================================
        // Build parameters
        // =============================================

        private CheatSheetViewModel _viewModel;
        public ModPopupBuilder WithViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        private Vector2 _position;
        private bool _hasPosition;
        public ModPopupBuilder WithPosition(Vector2 position)
        {
            this._position = position;
            this._hasPosition = true;
            return this;
        }
        public ModPopupBuilder WithoutPosition()
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
        public ModPopupController Build()
        {
            var popupBuilder = new PopupBuilder<TitleBarController, BodyController>()
                .WithPopupID(DIALOG_ID)
                .WithTitle(ModLocalization.GetString("SteamInput_titleHelp"))
                .WithIcon(SpritesTitleBar.GamepadIconSprite)
                .WithTitleBarBuilder(
                    new TitleBarBuilder().WithViewModel(_viewModel)
                )
                .WithContentBuilder(
                    new BodyBuilder().WithViewModel(_viewModel)
                )
                .WithSize(new Vector2(SteamInputPalette.WindowWidth, SteamInputPalette.WindowHeight));
            if( this._hasPosition )
            {
                popupBuilder = popupBuilder.WithPosition(this._position);
            }
            PopupController popupController = popupBuilder.Build();
            if( popupController == null ) return null;

            OverlayController overlayController = new OverlayBuilder()
                .Build();
            overlayController.transform.SetParent(popupController.GetGameObject().transform, false);
            overlayController.gameObject.SetActive(false);

            MenuBuilder.MenuController menuController = new MenuBuilder()
                .WithViewModel(_viewModel)
                .Build();
            menuController.transform.SetParent(popupController.GetGameObject().transform, false);
            menuController.gameObject.SetActive(false);
            
            return popupController
                .GetGameObject()
                .AddComponent<ModPopupController>()
                .WithViewModel(_viewModel)
                .WithPopupController(popupController)
                .WithOverlayController(overlayController)
                .WithMenuController(menuController)
                .Build();
        }
    }
}
