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

        private static Sprite _keyChipSprite;
        /// <summary>Sliced sprite for a key chip (.kkbd): fill + 1px border on all sides.</summary>
        public static Sprite KeyChipSprite
        {
            get
            {
                if (_keyChipSprite == null)
                {
                    _keyChipSprite = MakeChipSprite(
                        SteamInputPalette.RowKeyBgColor,
                        SteamInputPalette.RowKeyBorderColor);
                }
                return _keyChipSprite;
            }
        }

        private static Sprite _pressChipSprite;
        /// <summary>Sliced sprite for a press chip (.kpress): darker fill + 1px border.</summary>
        public static Sprite PressChipSprite
        {
            get
            {
                if (_pressChipSprite == null)
                {
                    _pressChipSprite = MakeChipSprite(
                        SteamInputPalette.RowPressBgColor,
                        SteamInputPalette.RowPressBorderColor);
                }
                return _pressChipSprite;
            }
        }

        private static Sprite MakeChipSprite(Color fill, Color border)
        {
            int thickness = Mathf.RoundToInt(SteamInputPalette.RowChipBorderThickness);
            int size = 2 * thickness + 1;
            var tex = SteamInputStyleTextures.MakeBorderTexture(fill, border, thickness);
            var sprite = Sprite.Create(
                tex,
                new Rect(0f, 0f, size, size),
                new Vector2(0.5f, 0.5f),
                100f,
                0u,
                SpriteMeshType.FullRect,
                new Vector4(thickness, thickness, thickness, thickness));
            sprite.hideFlags = HideFlags.HideAndDontSave;
            return sprite;
        }
    }
}
