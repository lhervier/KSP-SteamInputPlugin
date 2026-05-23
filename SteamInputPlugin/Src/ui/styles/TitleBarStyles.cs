using UnityEngine;

namespace com.github.lhervier.ksp.ui.styles
{
    /// <summary>Title bar text, badges, controller name, close button.</summary>
    public static class TitleBarStyles
    {
        public static GUIStyle Title { get; private set; }
        public static GUIStyle ActionSetBadge { get; private set; }
        public static GUIStyle ControllerName { get; private set; }
        public static GUIStyle CloseButton { get; private set; }
        public static GUIStyle MenuButton { get; private set; }

        internal static void Initialize(SteamInputStyleTextures textures)
        {
            Title = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                clipping = TextClipping.Clip
            };
            Title.normal.textColor = SteamInputPalette.TitleText;

            ActionSetBadge = new GUIStyle(GUI.skin.label)
            {
                fontSize = 10,
                padding = new RectOffset(5, 5, 2, 2),
                border = new RectOffset(1, 1, 1, 1),
                clipping = TextClipping.Clip,
                wordWrap = false,
                stretchWidth = false
            };
            ActionSetBadge.normal.textColor = SteamInputPalette.Accent;
            ActionSetBadge.normal.background = SteamInputStyleTextures.MakeBorderTexture(
                Color.clear,
                SteamInputPalette.BadgeBorder,
                (int)SteamInputPalette.BadgeBorderThickness);

            ControllerName = new GUIStyle(GUI.skin.label)
            {
                fontSize = 10,
                clipping = TextClipping.Clip,
                wordWrap = false,
                stretchWidth = false
            };
            ControllerName.normal.textColor = SteamInputPalette.ControllerName;

            CloseButton = new GUIStyle(GUI.skin.button)
            {
                fontSize = 13,
                fixedWidth = 20,
                fixedHeight = 20,
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0)
            };
            CloseButton.normal.background = textures.Button;
            CloseButton.normal.textColor = SteamInputPalette.DefaultButtonTextColor;
            CloseButton.hover.background = textures.ButtonHover;
            CloseButton.hover.textColor = Color.white;
            CloseButton.active.background = textures.Button;
            CloseButton.active.textColor = SteamInputPalette.DefaultButtonTextColor;

            MenuButton = new GUIStyle(CloseButton);
        }
    }
}
