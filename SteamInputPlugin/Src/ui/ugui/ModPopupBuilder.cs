using UnityEngine;
using com.github.lhervier.ksp.steaminput.ui.ugui.titleBar;
using com.github.lhervier.ksp.steaminput.ui.ugui.menu;
using com.github.lhervier.ksp.steaminput.ui.ugui.body;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.popup;
using com.github.lhervier.ksp.shared.ugui.overlay;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using com.github.lhervier.ksp.steaminput.ui.styles;

namespace com.github.lhervier.ksp.steaminput.ui.ugui
{
    public class ModPopupBuilder : IUGUIBuilder<ModPopupDialogController>
    {
        private const string DIALOG_ID = "SteamInputCheatSheetUGUI";
        
        private CheatSheetViewModel _viewModel;
        
        private Vector2 _position;
        
        // =============================================
        // Build parameters
        // =============================================

        public ModPopupBuilder ViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        public ModPopupBuilder Position(Vector2 position)
        {
            this._position = position;
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
            var popupController = new PopupBuilder<TitleBarController, BodyController>()
                .PopupID(DIALOG_ID)
                .Title(ModLocalization.GetString("SteamInput_titleHelp"))
                .Icon(SpritesTitleBar.GamepadIconSprite)
                .TitleBarBuilder(
                    new TitleBarBuilder().ViewModel(_viewModel)
                )
                .ContentBuilder(
                    new BodyBuilder().ViewModel(_viewModel)
                )
                .Size(new Vector2(SteamInputPalette.WindowWidth, SteamInputPalette.WindowHeight))
                .Position(this._position)
                .Build();
            if( popupController == null ) return null;

            OverlayController overlayController = new OverlayBuilder()
                .Build();
            overlayController.transform.SetParent(popupController.GetGameObject().transform, false);
            overlayController.gameObject.SetActive(false);

            MenuBuilder.MenuController menuController = new MenuBuilder()
                .ViewModel(_viewModel)
                .Build();
            menuController.transform.SetParent(popupController.GetGameObject().transform, false);
            menuController.gameObject.SetActive(false);
            
            return popupController
                .GetGameObject()
                .AddComponent<ModPopupDialogController>()
                .ViewModel(_viewModel)
                .PopupController(popupController)
                .OverlayController(overlayController)
                .MenuController(menuController)
                .Build();
        }
    }
}
