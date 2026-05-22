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

                var thickness = (int)SteamInputPalette.WindowBorderThickness;
                var size = 2 * thickness + 1;
                var tex = SteamInputStyleTextures.MakeBorderTexture(
                    SteamInputPalette.WindowBodyColor,
                    SteamInputPalette.WindowBorderColor,
                    thickness
                );
                _windowChromeSprite = Sprite.Create(
                    tex,
                    new Rect(0f, 0f, size, size),
                    new Vector2(0.5f, 0.5f),
                    100f,
                    0u,
                    SpriteMeshType.FullRect,
                    new Vector4(thickness, thickness, thickness, thickness));
                _windowChromeSprite.hideFlags = HideFlags.HideAndDontSave;
                return _windowChromeSprite;
            }
        }
    }
}
