using UnityEngine;
using com.github.lhervier.ksp.steaminput.ui.styles;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.sprites
{
    /// <summary>Sprites for the physical zones list rendered in the main body.</summary>
    internal static class SpritesActivators
    {
        private static Sprite _activatorInputSprite;
        /// <summary>Sliced sprite for a key chip (.kkbd): fill + 1px border on all sides.</summary>
        public static Sprite ActivatorInputSprite
        {
            get
            {
                if (_activatorInputSprite == null)
                {
                    _activatorInputSprite = SpritesGlobal.MakeChipSprite(
                        SteamInputPalette.ActivatorInputBgColor,
                        SteamInputPalette.ActivatorInputBorderColor,
                        SteamInputPalette.ActivatorInputBorderThickness);
                }
                return _activatorInputSprite;
            }
        }

        private static Sprite _activatorPressSprite;
        /// <summary>Sliced sprite for a press chip (.kpress): darker fill + 1px border.</summary>
        public static Sprite ActivatorPressSprite
        {
            get
            {
                if (_activatorPressSprite == null)
                {
                    _activatorPressSprite = SpritesGlobal.MakeChipSprite(
                        SteamInputPalette.ActivatorPressBgColor,
                        SteamInputPalette.ActivatorPressBorderColor,
                        SteamInputPalette.ActivatorPressBorderThickness);
                }
                return _activatorPressSprite;
            }
        }
    }
}
