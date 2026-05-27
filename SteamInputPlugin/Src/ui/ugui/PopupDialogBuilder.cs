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
        private const float ScreenX = 428f;
        private const float ScreenYFromTop = 20f;

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
                ScreenX, 
                ScreenYFromTop, 
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

            // Create the menu and the overlay events
            OverlayBuilder.OverlayController overlayController = null;
            MenuBuilder.MenuController menuController = null;
            Action toggleMenu = () => {
                if( menuController == null || overlayController == null )  return;
                bool willOpen = !menuController.gameObject.activeSelf;
                menuController.gameObject.SetActive(willOpen);
                overlayController.gameObject.SetActive(willOpen);
            };

            Action closeMenu = () => {
                if( menuController == null || overlayController == null )  return;
                menuController.gameObject.SetActive(false);
                overlayController.gameObject.SetActive(false);
            };

            // Add the body (scrollable content). First in z-order so the overlay/menu draw above it.
            BodyBuilder.BodyController bodyController = this._bodyBuilder.Create();
            bodyController.transform.SetParent(popupDialog.popupWindow.transform, false);

            // Add the overlay
            overlayController = _overlayBuilder.Create(closeMenu);
            overlayController.transform.SetParent(popupDialog.popupWindow.transform, false);
            overlayController.gameObject.SetActive(false);

            // Add the title bar
            TitleBarBuilder.TitleBarController titleBarController = this._titleBarBuilder.Create(toggleMenu);
            titleBarController.transform.SetParent(popupDialog.popupWindow.transform, false);

            // Add the menu
            menuController = this._menuBuilder.Create();
            menuController.transform.SetParent(popupDialog.popupWindow.transform, false);
            menuController.gameObject.SetActive(false);

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
    }
}