using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.ui.styles;
using UnityEngine.Events;
using com.github.lhervier.ksp.ui.ugui.styles;

namespace com.github.lhervier.ksp.ui.ugui
{
    public class PopupDialogBuilder
    {
        public static PopupDialog Create(
            float screenX, 
            float screenY, 
            float width, 
            float height,
            UnityAction onPopupDestroy
        )
        {
            // Creates a ultra minimal MultiOptionDialog. We will not use it.
            var pos = NormalizedWindowPos(screenX, screenY, width, height);
            var content = new DialogGUIVerticalLayout();
            MultiOptionDialog multiOptionDialog = new MultiOptionDialog(
                CheatSheetUGUIWindow.DIALOG_ID,
                string.Empty,
                string.Empty,
                HighLogic.UISkin,
                new Rect(pos.x, pos.y, SteamInputPalette.WindowWidth, height),
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
            popupDialog.onDestroy.AddListener(onPopupDestroy);

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
                image.color = SteamInputPalette.Body;
            }

            return popupDialog;
        }

        /// <summary>
        /// Normalized position from screen top-left, expressed as a percentage of the screen width and height.
        /// </summary>
        private static Vector2 NormalizedWindowPos(float screenX, float screenYFromTop, float width, float height)
        {
            var centerX = screenX + width * 0.5f;
            var centerY = Screen.height - screenYFromTop - height * 0.5f;
            return new Vector2(centerX / Screen.width, centerY / Screen.height);
        }
    }
}