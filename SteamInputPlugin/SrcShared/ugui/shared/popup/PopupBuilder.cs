using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.sprites;

namespace com.github.lhervier.ksp.ugui.shared.popup
{
    public class PopupBuilder<T, C> : IUGUIBuilder<PopupController> 
        where T : PopupTitleBarController
        where C : PopupContentController
    {
        private string _popupID;
        private IUGUIBuilder<T> _titleBarBuilder;
        private IUGUIBuilder<C> _contentBuilder;
        private bool _hasPosition = false;
        private Vector2 _initialPosition;
        
        public PopupBuilder(
            string popupID,
            IUGUIBuilder<T> titleBarBuilder,
            IUGUIBuilder<C> contentBuilder
        )
        {
            this._popupID = popupID;
            this._titleBarBuilder = titleBarBuilder;
            this._contentBuilder = contentBuilder;
        }

        public PopupBuilder(
            Vector2 initialPosition,
            string popupID,
            IUGUIBuilder<T> titleBarBuilder,
            IUGUIBuilder<C> contentBuilder
        ) : this(popupID, titleBarBuilder, contentBuilder)
        {
            this._initialPosition = initialPosition;
            this._hasPosition = true;
        }

        /// <summary>
        /// Spawn the cheat-sheet popup window and return its controller, or null if KSP failed to spawn
        /// it. The caller drives the window through the returned controller.
        /// </summary>
        public PopupController Create()
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
                this._popupID,
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
            
            PopupController controller = popupDialog.popupWindow.AddComponent<PopupController>();
            controller.BindPopupDialog(popupDialog);
            if (_hasPosition)
            {
                controller.InitializePosition(_initialPosition);
            }

            // Remove KSP default title
            var title = popupDialog.popupWindow.transform.Find("Title");
            title?.gameObject.SetActive(false);

            // Keep the window hidden until it has been positioned. KSP re-applies the initial
            // spawn position on every layout pass, so the window would otherwise flicker at the
            // default position before being moved to the saved one. The controller reveals it
            // (alpha 1) from Show(), once the layout has settled and the position has been applied.
            var canvasGroup = popupDialog.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                controller.BindCanvasGroup(canvasGroup);
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
            MonoBehaviour bodyController = this._contentBuilder.Create();
            bodyController.transform.SetParent(popupDialog.popupWindow.transform, false);

            // Add the title bar
            MonoBehaviour titleBarController = this._titleBarBuilder.Create();
            titleBarController.transform.SetParent(popupDialog.popupWindow.transform, false);

            return controller;
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
