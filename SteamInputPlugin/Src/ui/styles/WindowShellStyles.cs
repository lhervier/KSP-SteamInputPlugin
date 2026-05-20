using UnityEngine;

namespace com.github.lhervier.ksp.ui.styles
{
    /// <summary>Window chrome: border, body, title bar background.</summary>
    public static class WindowShellStyles
    {
        public static GUIStyle Window { get; private set; }
        public static GUIStyle Body { get; private set; }
        public static GUIStyle HeaderBar { get; private set; }

        internal static void Initialize(SteamInputStyleTextures textures)
        {
            Window = new GUIStyle(GUI.skin.window)
            {
                padding = new RectOffset(1, 1, 1, 1),
                border = new RectOffset(1, 1, 1, 1)
            };
            Window.normal.background = textures.Border;
            Window.onNormal.background = textures.Border;
            Window.focused.background = textures.Border;
            Window.onFocused.background = textures.Border;

            Body = new GUIStyle
            {
                padding = new RectOffset(8, 8, 6, 8)
            };
            Body.normal.background = textures.Body;

            HeaderBar = new GUIStyle
            {
                padding = new RectOffset(8, 6, 4, 4),
                fixedHeight = SteamInputPalette.TitleBarHeight
            };
            HeaderBar.normal.background = textures.Header;
        }
    }
}
