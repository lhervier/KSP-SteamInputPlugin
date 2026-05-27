using UnityEngine;

namespace com.github.lhervier.ksp.ui.styles
{
    /// <summary>Settings form controls: text field, toggle, menu button, dropdown box.</summary>
    public static class FormControlStyles
    {
        public static GUIStyle MenuButton { get; private set; }
        public static GUIStyle MenuBox { get; private set; }

        internal static void Initialize(SteamInputStyleTextures textures)
        {
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
