using UnityEngine;
using com.github.lhervier.ksp.ui.styles;

namespace com.github.lhervier.ksp.ui.ugui.styles
{
    /// <summary>Sprites for the physical zones list rendered in the main body.</summary>
    internal static class SpritesPhysicalZone
    {
        private const int HeaderBorderThickness = 1;

        private static Sprite _headerChromeSprite;
        /// <summary>
        /// Sliced sprite for a zone header: dark fill with a 1px line at the top and bottom,
        /// no left/right borders. Use with Image.Type.Sliced on a white-tinted Image.
        /// </summary>
        public static Sprite HeaderChromeSprite
        {
            get
            {
                if (_headerChromeSprite != null)
                {
                    return _headerChromeSprite;
                }

                var height = 2 * HeaderBorderThickness + 1;
                var tex = SteamInputStyleTextures.MakeHorizontalBordersTexture(
                    SteamInputPalette.MainZoneHeaderColor,
                    SteamInputPalette.ZoneSeparator,
                    HeaderBorderThickness
                );
                _headerChromeSprite = Sprite.Create(
                    tex,
                    new Rect(0f, 0f, 1f, height),
                    new Vector2(0.5f, 0.5f),
                    100f,
                    0u,
                    SpriteMeshType.FullRect,
                    // (left, bottom, right, top) — no horizontal borders, 1px top + bottom
                    new Vector4(0f, HeaderBorderThickness, 0f, HeaderBorderThickness));
                _headerChromeSprite.hideFlags = HideFlags.HideAndDontSave;
                return _headerChromeSprite;
            }
        }
    }
}
