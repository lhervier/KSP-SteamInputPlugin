using UnityEngine;

namespace com.github.lhervier.ksp.ui.styles
{
    /// <summary>Settings form controls: text field, toggle, menu button, dropdown box.</summary>
    public static class FormControlStyles
    {
        public static GUIStyle TextField { get; private set; }
        public static GUIStyle Toggle { get; private set; }
        public static GUIStyle MenuButton { get; private set; }
        public static GUIStyle MenuBox { get; private set; }

        internal static void Initialize(SteamInputStyleTextures textures)
        {
            TextField = new GUIStyle(GUI.skin.textField)
            {
                fontSize = 12,
                padding = new RectOffset(4, 4, 3, 3)
            };
            TextField.normal.textColor = SteamInputPalette.TitleText;
            TextField.normal.background = textures.FieldBackground;
            TextField.focused.background = textures.FieldBackground;
            TextField.focused.textColor = SteamInputPalette.TitleText;

            Toggle = new GUIStyle(GUI.skin.toggle)
            {
                fontSize = 12,
                padding = new RectOffset(20, 0, 2, 0)
            };
            Toggle.normal.textColor = SteamInputPalette.Label;
            Toggle.onNormal.textColor = SteamInputPalette.Label;
            Toggle.hover.textColor = SteamInputPalette.TitleText;

            MenuButton = new GUIStyle(TitleBarStyles.CloseButton)
            {
                fixedWidth = 100
            };

            MenuBox = new GUIStyle
            {
                padding = new RectOffset(4, 4, 4, 4)
            };
            MenuBox.normal.background = textures.MenuBox;
        }
    }
}
