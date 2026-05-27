using UnityEngine;

namespace com.github.lhervier.ksp.ui.styles
{
    /// <summary>Title bar text, badges, controller name, close button.</summary>
    public static class TitleBarStyles
    {
        public static GUIStyle CloseButton { get; private set; }
        
        internal static void Initialize(SteamInputStyleTextures textures)
        {
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
        }
    }
}
