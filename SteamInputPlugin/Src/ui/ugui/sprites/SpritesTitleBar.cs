using UnityEngine;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.shared.ugui.styles;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.sprites
{
    /// <summary>Applies ksp_cheatsheet mockup colors to a KSP PopupDialog shell.</summary>
    internal static class SpritesTitleBar
    {
        private static Sprite _actionGroupBorderSprite;
        public static Sprite ActionGroupBorderSprite
        {
            get
            {
                if (_actionGroupBorderSprite != null)
                {
                    return _actionGroupBorderSprite;
                }

                var thickness = (int) PopupPalette.TitleBarActionGroupBorderThickness;
                var size = 2 * thickness + 1;
                var tex = SteamInputTextures.MakeBorderTexture(
                    Color.clear,
                    SteamInputPalette.TitleBarActionGroupBorderColor,
                    thickness
                );
                _actionGroupBorderSprite = Sprite.Create(
                    tex,
                    new Rect(0f, 0f, size, size),
                    new Vector2(0.5f, 0.5f),
                    100f,
                    0u,
                    SpriteMeshType.FullRect,
                    new Vector4(thickness, thickness, thickness, thickness));
                _actionGroupBorderSprite.hideFlags = HideFlags.HideAndDontSave;
                return _actionGroupBorderSprite;
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
