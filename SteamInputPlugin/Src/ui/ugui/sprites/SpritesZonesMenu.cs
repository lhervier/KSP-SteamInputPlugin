using UnityEngine;
using com.github.lhervier.ksp.ui.styles;

namespace com.github.lhervier.ksp.ui.ugui.sprites
{
    /// <summary>Sprites for the zones menu (the dropdown opened from the title bar's "..." button).</summary>
    internal static class SpritesZonesMenu
    {
        private static Sprite _chromeSprite;
        public static Sprite ChromeSprite
        {
            get
            {
                if (_chromeSprite != null)
                {
                    return _chromeSprite;
                }

                var thickness = (int) SteamInputPalette.MenuThickness;
                var size = 2 * thickness + 1;
                var tex = SteamInputStyleTextures.MakeBorderTexture(
                    SteamInputPalette.MenuBackgroundColor,
                    SteamInputPalette.WindowBorderColor,
                    thickness
                );
                _chromeSprite = Sprite.Create(
                    tex,
                    new Rect(0f, 0f, size, size),
                    new Vector2(0.5f, 0.5f),
                    100f,
                    0u,
                    SpriteMeshType.FullRect,
                    new Vector4(thickness, thickness, thickness, thickness));
                _chromeSprite.hideFlags = HideFlags.HideAndDontSave;
                return _chromeSprite;
            }
        }
    }
}
