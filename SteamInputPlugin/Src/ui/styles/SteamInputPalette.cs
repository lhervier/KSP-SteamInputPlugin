using UnityEngine;

namespace com.github.lhervier.ksp.ui.styles
{
    /// <summary>Layout metrics and colors from ksp_cheatsheet mockup.</summary>
    public static class SteamInputPalette
    {
        // ==============================================================
        // Default settings
        // ==============================================================
        
        // Default values
        public const float DefaultPaddingLeft = 8f;
        public const float DefaultPaddingRight = 8f;
        public const float DefaultPaddingTop = 5f;
        public const float DefaultPaddingBottom = 5f;
        public const float DefaultSpacing = 6f;
        
        // Default colors
        public static readonly Color DefaultAccentColor = new Color(141f / 255f, 190f / 255f, 69f / 255f);
        public static readonly Color DefaultLabelColor = new Color(187f / 255f, 187f / 255f, 187f / 255f);
        public static readonly Color DefaultFieldBackgroundColor = new Color(42f / 255f, 42f / 255f, 42f / 255f);
        public static readonly Color DefaultSeparatorColor = new Color(42f / 255f, 42f / 255f, 42f / 255f);

        // Icons
        public const int DefaultIconSize = 18;
        
        // Buttons
        public const float DefaultButtonSize = 18f;
        public static readonly Color DefaultButtonColor = new Color(42f / 255f, 42f / 255f, 42f / 255f);
        public static readonly Color DefaultButtonHoverColor = new Color(56f / 255f, 56f / 255f, 56f / 255f);
        public static readonly Color DefaultButtonTextColor = new Color(187f / 255f, 187f / 255f, 187f / 255f);
        public static readonly Color DefaultButtonDisabledTextColor = new Color(187f / 255f, 187f / 255f, 187f / 255f, 0.25f);
        
        // Checkbox
        public const float DefaultCheckboxSize = 12f;
        public const float DefaultCheckmarkInset = 2f;
                
        // ===============================================================
        // Main Window
        // ===============================================================
        public const float WindowWidth = 400f;
        public const float WindowHeight = 320f;
        public const float WindowBorderThickness = 1f;
        // Colors
        public static readonly Color WindowBodyColor = new Color(20f / 255f, 20f / 255f, 20f / 255f);
        public static readonly Color WindowBorderColor = new Color(85f / 255f, 85f / 255f, 85f / 255f);
        
        // ===============================================================
        // Title bar
        // ===============================================================
        public const float TitleBarHeight = 28f;
        public const float TitleBarSeparatorHeight = 1f;
        public const string TitleBarGamepadIconPath = "SteamInput/Textures/gamepad_icon";
        public const float TitleBarActionGroupBorderThickness = 1f;
        
        // Colors
        public static readonly Color TitleBarLabelColor = new Color(232f / 255f, 232f / 255f, 232f / 255f);
        public static readonly Color TitleBarSeparatorColor = new Color(68f / 255f, 68f / 255f, 68f / 255f);
        public static readonly Color TitleBarActionGroupBorderColor = new Color(74f / 255f, 110f / 255f, 32f / 255f);
        public static readonly Color TitleBarControllerNameColor = new Color(85f / 255f, 85f / 255f, 85f / 255f);
        public static readonly Color TitleBarButtonColor = new Color(56f / 255f, 56f / 255f, 56f / 255f);
        public static readonly Color TitleBarButtonHoverColor = new Color(72f / 255f, 72f / 255f, 72f / 255f);

        // ====================================================================
        // Menu
        // ====================================================================

        public const float MenuWidth = 220f;
        public const float MenuSpacing = 3f;
        public const float MenuPaddingLeft = 10f;
        public const float MenuPaddingRight = 10f;
        public const float MenuPaddingTop = 4f;
        public const float MenuPaddingBottom = 4f;
        public const float MenuArrowsSpacing = 2f;

        public static readonly Color MenuTitleColor = new Color(85f / 255f, 85f / 255f, 85f / 255f);
        



        
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
        
        public static readonly Color FieldBackground = new Color(42f / 255f, 42f / 255f, 42f / 255f);
        public static readonly Color ButtonHover = new Color(72f / 255f, 72f / 255f, 72f / 255f);
        /// <summary>.kmenu background (#1e1e1e) — lighter than window body (#141414).</summary>
        public static readonly Color ZonesMenuBackground = new Color(30f / 255f, 30f / 255f, 30f / 255f);
        public static readonly Color MenuBox = ZonesMenuBackground;
        
        /// <summary>.kmenu-sep (#2a2a2a).</summary>
        

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
