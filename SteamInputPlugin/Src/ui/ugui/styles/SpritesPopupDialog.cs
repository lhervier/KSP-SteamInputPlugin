using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.ui.styles;

namespace com.github.lhervier.ksp.ui.ugui.styles
{
    /// <summary>Applies ksp_cheatsheet mockup colors to a KSP PopupDialog shell.</summary>
    internal static class SpritesPopupDialog
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
    }
}
