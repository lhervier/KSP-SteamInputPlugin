using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.ui.styles;

namespace com.github.lhervier.ksp.ui.ugui
{
    /// <summary>Applies ksp_cheatsheet mockup colors to a KSP PopupDialog shell.</summary>
    internal static class CheatSheetUGUIChrome
    {
        private static Sprite windowChromeSprite;
        private static Sprite WindowChromeSprite
        {
            get
            {
                if (windowChromeSprite != null)
                {
                    return windowChromeSprite;
                }

                var tex = SteamInputStyleTextures.MakeBorderTexture(
                    SteamInputPalette.Body,
                    SteamInputPalette.Border
                );
                windowChromeSprite = Sprite.Create(
                    tex,
                    new Rect(0f, 0f, 3f, 3f),
                    new Vector2(0.5f, 0.5f),
                    100f,
                    0u,
                    SpriteMeshType.FullRect,
                    new Vector4(1f, 1f, 1f, 1f));
                windowChromeSprite.hideFlags = HideFlags.HideAndDontSave;
                return windowChromeSprite;
            }
        }

        private static Sprite bodyFillSprite;
        private static Sprite BodyFillSprite
        {
            get
            {
                if (bodyFillSprite != null)
                {
                    return bodyFillSprite;
                }

                var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                tex.SetPixel(0, 0, Color.white);
                tex.Apply();
                tex.filterMode = FilterMode.Point;
                tex.hideFlags = HideFlags.HideAndDontSave;
                bodyFillSprite = Sprite.Create(tex, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 100f);
                bodyFillSprite.hideFlags = HideFlags.HideAndDontSave;
                return bodyFillSprite;
            }
        }

        public static void Apply(PopupDialog dialog)
        {
            if (dialog == null || dialog.popupWindow == null)
            {
                return;
            }

            // Set background color as non-transparent
            var canvasGroup = dialog.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }

            // Set windows border color 
            var windowGo = dialog.popupWindow;
            var windowImage = windowGo.GetComponent<Image>();
            if (windowImage != null)
            {
                windowImage.sprite = WindowChromeSprite;
                windowImage.type = Image.Type.Sliced;
                windowImage.color = Color.white;
                windowImage.raycastTarget = true;
            }

            // Hide KSP title bar
            var title = windowGo.transform.Find("Title");
            if (title != null)
            {
                title.gameObject.SetActive(false);
            }

            // Get the custom title bar
            var customTitleBar = windowGo.transform.Find(CheatSheetUGUIWindow.TITLEBAR_OBJECT_NAME);

            // Set windows background color
            foreach (var image in windowGo.GetComponentsInChildren<Image>(true))
            {
                if (image == windowImage)
                {
                    continue;
                }

                if (customTitleBar != null && image.transform.IsChildOf(customTitleBar))
                {
                    continue;
                }

                image.sprite = BodyFillSprite;
                image.type = Image.Type.Simple;
                image.color = SteamInputPalette.Body;
            }
        }

        
    }
}
