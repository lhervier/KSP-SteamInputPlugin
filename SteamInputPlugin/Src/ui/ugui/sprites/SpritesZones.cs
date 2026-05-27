using UnityEngine;
using com.github.lhervier.ksp.ui.styles;

namespace com.github.lhervier.ksp.ui.ugui.sprites
{
    /// <summary>Sprites for the physical zones list rendered in the main body.</summary>
    internal static class SpritesZones
    {
        private static Sprite _zoneHeaderChromeSprite;
        /// <summary>
        /// Sliced sprite for a zone header: dark fill with a 1px line at the top and bottom,
        /// no left/right borders. Use with Image.Type.Sliced on a white-tinted Image.
        /// </summary>
        public static Sprite ZoneHeaderChromeSprite
        {
            get
            {
                if (_zoneHeaderChromeSprite != null)
                {
                    return _zoneHeaderChromeSprite;
                }

                var height = 2 * SteamInputPalette.ZoneHeaderBorderThickness + 1;
                var tex = SteamInputStyleTextures.MakeHorizontalBordersTexture(
                    SteamInputPalette.ZoneHeaderColor,
                    SteamInputPalette.ZoneSeparatorColor,
                    SteamInputPalette.ZoneHeaderBorderThickness
                );
                _zoneHeaderChromeSprite = Sprite.Create(
                    tex,
                    new Rect(0f, 0f, 1f, height),
                    new Vector2(0.5f, 0.5f),
                    100f,
                    0u,
                    SpriteMeshType.FullRect,
                    // (left, bottom, right, top) — no horizontal borders, 1px top + bottom
                    new Vector4(0f, SteamInputPalette.ZoneHeaderBorderThickness, 0f, SteamInputPalette.ZoneHeaderBorderThickness));
                _zoneHeaderChromeSprite.hideFlags = HideFlags.HideAndDontSave;
                return _zoneHeaderChromeSprite;
            }
        }

        private static Sprite _comboSprite;
        /// <summary>Sliced sprite for a key chip (.kkbd): fill + 1px border on all sides.</summary>
        public static Sprite ComboSprite
        {
            get
            {
                if (_comboSprite == null)
                {
                    _comboSprite = SpritesGlobal.MakeChipSprite(
                        SteamInputPalette.ActivatorInputBgColor,
                        SteamInputPalette.ActivatorInputBorderColor,
                    SteamInputPalette.ZoneHeaderBorderThickness);
                }
                return _comboSprite;
            }
        }
    }
}
