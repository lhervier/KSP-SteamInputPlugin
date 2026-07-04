using UnityEngine;
using com.github.lhervier.ksp.steaminput.ui.styles;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.sprites
{
    /// <summary>Applies ksp_cheatsheet mockup colors to a KSP PopupDialog shell.</summary>
    internal static class SpritesTitleBar
    {
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
                    SteamInputPalette.TitleBarGamepadIconPath, 
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
