using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.ui.styles;

namespace com.github.lhervier.ksp.ui.ugui
{
    /// <summary>Applies ksp_cheatsheet mockup colors to a KSP PopupDialog shell.</summary>
    internal static class CheatSheetUGUIChrome
    {
        private static Sprite _windowChromeSprite;
        public static Sprite WindowChromeSprite
        {
            get
            {
                if (_windowChromeSprite != null)
                {
                    return _windowChromeSprite;
                }

                var tex = SteamInputStyleTextures.MakeBorderTexture(
                    SteamInputPalette.Body,
                    SteamInputPalette.Border
                );
                _windowChromeSprite = Sprite.Create(
                    tex,
                    new Rect(0f, 0f, 3f, 3f),
                    new Vector2(0.5f, 0.5f),
                    100f,
                    0u,
                    SpriteMeshType.FullRect,
                    new Vector4(1f, 1f, 1f, 1f));
                _windowChromeSprite.hideFlags = HideFlags.HideAndDontSave;
                return _windowChromeSprite;
            }
        }

        private static Sprite _fillSprite;
        public static Sprite FillSprite
        {
            get
            {
                if (_fillSprite != null)
                {
                    return _fillSprite;
                }

                var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                tex.SetPixel(0, 0, Color.white);
                tex.Apply();
                tex.filterMode = FilterMode.Point;
                tex.hideFlags = HideFlags.HideAndDontSave;
                _fillSprite = Sprite.Create(tex, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 100f);
                _fillSprite.hideFlags = HideFlags.HideAndDontSave;
                return _fillSprite;
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
    }
}
