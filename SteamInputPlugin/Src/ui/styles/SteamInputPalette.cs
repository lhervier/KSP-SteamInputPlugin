using UnityEngine;

namespace com.github.lhervier.ksp.ui.styles
{
    /// <summary>Layout metrics and colors from ksp_cheatsheet mockup.</summary>
    public static class SteamInputPalette
    {
        // ==============================================================
        // Default settings
        // ==============================================================
        public const float DefaultPaddingLeft = 8f;
        public const float DefaultPaddingRight = 8f;
        public const float DefaultPaddingTop = 5f;
        public const float DefaultPaddingBottom = 5f;
        public const float DefaultSpacing = 6f;
        public const int DefaultIconSize = 18;
        public const float DefaultButtonSize = 18f;

        // ===============================================================
        // Main Window
        // ===============================================================
        public const float WindowWidth = 400f;
        public const float WindowHeight = 320f;
        public const float WindowBorderThickness = 1f;
        public static readonly Color WindowBodyColor = new Color(20f / 255f, 20f / 255f, 20f / 255f);
        public static readonly Color WindowBorderColor = new Color(85f / 255f, 85f / 255f, 85f / 255f);
        
        // ===============================================================
        // Title bar
        // ===============================================================
        public const float TitleBarHeight = 28f;
        public const float TitleBarSeparatorHeight = 1f;
        public const string TitleBarGamepadIconPath = "SteamInput/Textures/gamepad_icon";
        public const float TitleBarActionGroupBorderThickness = 1f;
        public static readonly Color TitleBarLabelColor = new Color(232f / 255f, 232f / 255f, 232f / 255f);
        public static readonly Color TitleBarSeparatorColor = new Color(68f / 255f, 68f / 255f, 68f / 255f);
        public static readonly Color TitleBarActionGroupLabelColor = new Color(141f / 255f, 190f / 255f, 69f / 255f);
        public static readonly Color TitleBarActionGroupBorderColor = new Color(74f / 255f, 110f / 255f, 32f / 255f);
        
        
        public const float IconTitleGap = 6f;
        public static readonly Color Header = new Color(46f / 255f, 46f / 255f, 46f / 255f);
        public static readonly Color TitleText = new Color(232f / 255f, 232f / 255f, 232f / 255f);
        public static readonly Color Label = new Color(204f / 255f, 204f / 255f, 204f / 255f);
        public static readonly Color Muted = new Color(136f / 255f, 136f / 255f, 136f / 255f);
        public static readonly Color ControllerName = new Color(85f / 255f, 85f / 255f, 85f / 255f);
        public static readonly Color Accent = new Color(141f / 255f, 190f / 255f, 69f / 255f);
        public static readonly Color BadgeBorder = new Color(74f / 255f, 110f / 255f, 32f / 255f);
        public const float BadgeBorderThickness = 1f;
        public static readonly Color Warn = new Color(0.95f, 0.82f, 0.23f);
        public static readonly Color Button = new Color(56f / 255f, 56f / 255f, 56f / 255f);
        public static readonly Color ButtonText = new Color(187f / 255f, 187f / 255f, 187f / 255f);
        public static readonly Color FieldBackground = new Color(42f / 255f, 42f / 255f, 42f / 255f);
        public static readonly Color ButtonHover = new Color(72f / 255f, 72f / 255f, 72f / 255f);
        /// <summary>.kmenu background (#1e1e1e) — lighter than window body (#141414).</summary>
        public static readonly Color ZonesMenuBackground = new Color(30f / 255f, 30f / 255f, 30f / 255f);
        public static readonly Color MenuBox = ZonesMenuBackground;
        public static readonly Color MenuTitle = new Color(85f / 255f, 85f / 255f, 85f / 255f);
        /// <summary>.kmenu-sep (#2a2a2a).</summary>
        public static readonly Color MenuSeparator = new Color(42f / 255f, 42f / 255f, 42f / 255f);

        // Physical zones (ksp_cheatsheet *.html — .kzone, .kzh, .kstate)
        public static readonly Color ZoneHeaderBg = new Color(26f / 255f, 26f / 255f, 26f / 255f);
        public static readonly Color ZoneSeparator = new Color(34f / 255f, 34f / 255f, 34f / 255f);
        public static readonly Color ZoneName = new Color(221f / 255f, 221f / 255f, 221f / 255f);
        public static readonly Color SectionNormal = new Color(72f / 255f, 72f / 255f, 72f / 255f);
        public static readonly Color SectionModeshift = new Color(176f / 255f, 115f / 255f, 24f / 255f);
        public const float ZoneHeaderHeight = 22f;
        public const float ZoneSectionSpacing = 2f;
        public const float ZoneBodyPaddingBottom = 5f;
    }
}
