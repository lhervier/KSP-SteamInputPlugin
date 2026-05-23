using UnityEngine;

namespace com.github.lhervier.ksp.ui.styles
{
    /// <summary>Zones visibility menu (.kmenu) from ksp_cheatsheet mockup.</summary>
    public static class ZonesMenuStyles
    {
        public const float PanelWidth = 220f;
        public const float PanelTop = 24f;
        public const float PanelBorder = 1f;
        /// <summary>Fallback when toggle button layout rect is not available yet.</summary>
        public const float PanelRightInset = 8f;
        public const float SeparatorMargin = 3f;
        public const float SeparatorHeight = 1f;
        public const float ContentPlaceholderHeight = 180f;
        public const float PanelBottomPadding = 4f;

        /// <summary>Inner title band height (28px header minus 4px padding top and bottom).</summary>
        public const float TitleContentHeight = 20f;

        public static float TitleHeaderHeight
        {
            get { return SteamInputPalette.TitleBarHeight; }
        }

        public static GUIStyle Panel { get; private set; }
        public static GUIStyle Title { get; private set; }
        public static GUIStyle Separator { get; private set; }

        internal static void Initialize(SteamInputStyleTextures textures)
        {
            // GUI.Box + 9-slice — do not use this style with GUILayout.BeginArea (stretches the 3×3).
            Panel = new GUIStyle
            {
                border = new RectOffset(1, 1, 1, 1),
                padding = new RectOffset(0, 0, 0, 0)
            };
            Panel.normal.background = SteamInputStyleTextures.MakeBorderTexture(
                SteamInputPalette.ZonesMenuBackground,
                SteamInputPalette.WindowBorderColor,
                (int)PanelBorder);

            Title = new GUIStyle(GUI.skin.label)
            {
                fontSize = 10,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(10, 10, 0, 0),
                margin = new RectOffset(0, 0, 0, 0),
                clipping = TextClipping.Clip,
                wordWrap = false,
                stretchWidth = true
            };
            Title.normal.textColor = SteamInputPalette.MenuTitleColor;

            Separator = new GUIStyle
            {
                fixedHeight = SeparatorHeight,
                margin = new RectOffset(0, 0, 0, 0)
            };
            Separator.normal.background = textures.MenuSeparator;
        }

        public static float SeparatorBlockHeight
        {
            get { return SeparatorMargin + SeparatorHeight + SeparatorMargin; }
        }

        public static Rect ContentRect(Rect panelRect)
        {
            return new Rect(
                panelRect.x + PanelBorder,
                panelRect.y + PanelBorder,
                panelRect.width - PanelBorder * 2f,
                panelRect.height - PanelBorder * 2f);
        }
    }
}
