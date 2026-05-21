using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.ui.styles;

namespace com.github.lhervier.ksp.ui.ugui
{
    /// <summary>Applies ksp_cheatsheet mockup colors to a KSP PopupDialog shell.</summary>
    internal static class CheatSheetUGUIChrome
    {
        private static Sprite windowChromeSprite;
        public static Sprite WindowChromeSprite
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
        public static Sprite BodyFillSprite
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

        private static Sprite _gamepadIconSprite;
        public static Sprite GamepadIconSprite
        {
            get
            {
                if (_gamepadIconSprite != null)
                {
                    return _gamepadIconSprite;
                }

                if (GameDatabase.Instance == null)
                {
                    return null;
                }

                var tex = GameDatabase.Instance.GetTexture(
                    SteamInputPalette.GamepadIconPath, 
                    false
                );
                if (tex == null)
                {
                    return null;
                }

                _gamepadIconSprite = Sprite.Create(
                    tex,
                    new Rect(0f, 0f, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f),
                    100f
                );
                _gamepadIconSprite.hideFlags = HideFlags.HideAndDontSave;
                return _gamepadIconSprite;
            }
        }

        private static Sprite _headerFillSprite;
        public static Sprite HeaderFillSprite
        {
            get
            {
                if (_headerFillSprite != null)
                {
                    return _headerFillSprite;
                }

                var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                tex.SetPixel(0, 0, Color.white);
                tex.Apply();
                tex.filterMode = FilterMode.Point;
                tex.hideFlags = HideFlags.HideAndDontSave;
                _headerFillSprite = Sprite.Create(tex, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 100f);
                _headerFillSprite.hideFlags = HideFlags.HideAndDontSave;
                return _headerFillSprite;
            }
        }
    }
}
