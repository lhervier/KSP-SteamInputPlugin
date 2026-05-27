using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.ui.styles;
using UnityEngine.Events;
using com.github.lhervier.ksp.ui.ugui.sprites;
using com.github.lhervier.ksp.ui.ugui.titleBar;
using System;
using com.github.lhervier.ksp.ui.ugui.menu;
using com.github.lhervier.ksp.ui.ugui.body;

namespace com.github.lhervier.ksp.ui.ugui
{
    public class PopupDialogBuilder
    {
        private const string DIALOG_ID = "SteamInputCheatSheetUGUI";
        private CheatSheetViewModel _viewModel;
        private TitleBarBuilder _titleBarBuilder;
        private OverlayBuilder _overlayBuilder;
        private MenuBuilder _menuBuilder;
        private BodyBuilder _bodyBuilder;

        public PopupDialogBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._titleBarBuilder = new TitleBarBuilder(viewModel);
            this._overlayBuilder = new OverlayBuilder(viewModel);
            this._menuBuilder = new MenuBuilder(viewModel);
            this._bodyBuilder = new BodyBuilder(viewModel);
        }

        public PopupDialog CreatePopupDialog()
        {
            // Creates a ultra minimal MultiOptionDialog. We will not use it.
            var pos = NormalizedWindowPos(
                SteamInputPalette.WindowInitialPositionX, 
                SteamInputPalette.WindowInitialPositionY, 
                SteamInputPalette.WindowWidth,
                SteamInputPalette.WindowHeight
            );
            var content = new DialogGUIVerticalLayout();
            MultiOptionDialog multiOptionDialog = new MultiOptionDialog(
                DIALOG_ID,
                string.Empty,
                string.Empty,
                HighLogic.UISkin,
                pos,
                new DialogGUIBase[]
                {
                    new DialogGUIBox(null, -1, -1, () => true, content)
                }
            );
            
            // Creates the popup dialog
            PopupDialog popupDialog = PopupDialog.SpawnPopupDialog(
                multiOptionDialog,
                true,
                HighLogic.UISkin,
                false,
                string.Empty
            );
            if( popupDialog == null || popupDialog.popupWindow == null )
            {
                return null;
            }
            PopupDialogController controller = popupDialog.popupWindow.AddComponent<PopupDialogController>();
            controller.Initialize(_viewModel);
            controller.BindOverlayBuilder(_overlayBuilder);
            controller.BindMenuBuilder(_menuBuilder);

            // Remove KSP default title
            var title = popupDialog.popupWindow.transform.Find("Title");
            if (title != null)
            {
                title.gameObject.SetActive(false);
            }

            // Set background color as non-transparent
            var canvasGroup = popupDialog.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }

            // Set windows border color 
            var windowGo = popupDialog.popupWindow;
            var windowImage = windowGo.GetComponent<Image>();
            if (windowImage != null)
            {
                windowImage.sprite = SpritesPopupDialog.WindowChromeSprite;
                windowImage.type = Image.Type.Sliced;
                windowImage.color = Color.white;

                // Raycast to prevent mouse event to be sent to the game
                windowImage.raycastTarget = true;
            }

            // Set windows background color
            foreach (var image in windowGo.GetComponentsInChildren<Image>(true))
            {
                if (image == windowImage)
                {
                    continue;
                }

                image.sprite = SpritesGlobal.FillSprite;
                image.type = Image.Type.Simple;
                image.color = SteamInputPalette.WindowBodyColor;
            }

            // Add the body (scrollable content). First in z-order so the overlay/menu draw above it.
            BodyBuilder.BodyController bodyController = this._bodyBuilder.Create();
            bodyController.transform.SetParent(popupDialog.popupWindow.transform, false);

            // Add the title bar
            TitleBarBuilder.TitleBarController titleBarController = this._titleBarBuilder.Create();
            titleBarController.transform.SetParent(popupDialog.popupWindow.transform, false);

            return popupDialog;
        }

        /// <summary>
        /// Normalized position from screen top-left, expressed as a percentage of the screen width and height.
        /// </summary>
        private static Rect NormalizedWindowPos(float screenX, float screenYFromTop, float width, float height)
        {
            var centerX = screenX + width * 0.5f;
            var centerY = Screen.height - screenYFromTop - height * 0.5f;
            return new Rect(centerX / Screen.width, centerY / Screen.height, width, height);
        }

        public class PopupDialogController : BaseSteamInputController
        {
            private OverlayBuilder _overlayBuilder;
            private MenuBuilder _menuBuilder;

            private OverlayBuilder.OverlayController _overlayController = null;
            private MenuBuilder.MenuController _menuController = null;

            public void BindOverlayBuilder(OverlayBuilder builder)
            {
                this._overlayBuilder = builder;
            }

            public void BindMenuBuilder(MenuBuilder builder)
            {
                this._menuBuilder = builder;
            }

            public void Start()
            {
                ViewModel?.OnShowMenu.Add(OnShowMenu);
                if( ViewModel != null )
                {
                    OnShowMenu(ViewModel.MenuDisplayed);
                }
            }

            public void OnDestroy()
            {
                ViewModel?.OnShowMenu.Remove(OnShowMenu);
            }

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
        }
    }
}