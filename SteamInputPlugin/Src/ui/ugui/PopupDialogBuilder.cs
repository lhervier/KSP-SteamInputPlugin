using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.ui.styles;
using UnityEngine.Events;
using com.github.lhervier.ksp.ui.ugui.styles;
using com.github.lhervier.ksp.ui.ugui.titleBar;

namespace com.github.lhervier.ksp.ui.ugui
{
    public class PopupDialogBuilder
    {
        private const string DIALOG_ID = "SteamInputCheatSheetUGUI";
        private const float ScreenX = 428f;
        private const float ScreenYFromTop = 20f;

        private CheatSheetViewModel _viewModel;
        private TitleBarBuilder _titleBarBuilder;

        public PopupDialogBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._titleBarBuilder = new TitleBarBuilder(viewModel);
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

            // Add the title bar
            GameObject titleBarGo = this._titleBarBuilder.CreateGameObject();
            titleBarGo.transform.SetParent(popupDialog.popupWindow.transform, false);

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