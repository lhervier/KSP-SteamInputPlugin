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
        public const float WindowInitialPositionX = 428f;
        public const float WindowInitialPositionY = 20f;
        public const float WindowWidth = 350f;
        public const float WindowHeight = 400f;
        public const float WindowBorderThickness = 1f;
        
        // Colors
        public static readonly Color WindowBodyColor = new Color(20f / 255f, 20f / 255f, 20f / 255f);
        public static readonly Color WindowBorderColor = new Color(85f / 255f, 85f / 255f, 85f / 255f);
        
        // ===============================================================
        // Main content
        // ===============================================================

        public const float MainScrollbarWidth = 8f;
        public const float MainPlaceholderHeight = 800f;
        
        // ================================================================
        // Physical zones on the controller
        // ================================================================
        
        // Zone header
        public const float ZoneHeaderHeight = 22f;
        public const int ZoneHeaderBorderThickness = 1;
        
        public static readonly Color ZoneHeaderColor = new Color(26f / 255f, 26f / 255f, 26f / 255f);
        public static readonly Color ZoneSeparatorColor = new Color(34f / 255f, 34f / 255f, 34f / 255f);
        public static readonly Color ZoneNameColor = new Color(221f / 255f, 221f / 255f, 221f / 255f);
        
        // Zone body
        public const float ZoneBodySpacing = 2f;
        
        // ================================================================
        // Modes on an zone (= modes : normal or modeshift)
        // ================================================================
        public const float ModeSpacing = 2f;
        public const int ModeLabelFontSize = 10;

        public static readonly Color ModeShiftColor = new Color(176f / 255f, 115f / 255f, 24f / 255f);
        public static readonly Color ModeNormalColor = new Color(72f / 255f, 72f / 255f, 72f / 255f);
        
        // ===============================================================
        // Activator = Button A, dpad north, click, ...
        // ===============================================================

        public const float ActivatorPaddingV = 2f;
        public const float ActivatorSeparatorPaddingH = 4f;
        
        // Input icon
        public const float ActivatorInputSpacing = 3f;
        public const float ActivatorInputMinWidth = 100f;
        public const int ActivatorInputFontSize = 11;
        public const float ActivatorInputPaddingH = 5f;
        public const int ActivatorInputBorderThickness = 1;
            
                
        public static readonly Color ActivatorInputTextColor = new Color(232f / 255f, 232f / 255f, 232f / 255f);
        public static readonly Color ActivatorInputBgColor = new Color(42f / 255f, 42f / 255f, 42f / 255f);
        public static readonly Color ActivatorInputBorderColor = new Color(85f / 255f, 85f / 255f, 85f / 255f);

        // Long press chip
        public const int ActivatorPressFontSize = 9;
        public const int ActivatorPressBorderThickness = 1;
        
        public static readonly Color ActivatorPressBgColor = new Color(34f / 255f, 34f / 255f, 34f / 255f);
        public static readonly Color ActivatorPressTextColor = new Color(85f / 255f, 85f / 255f, 85f / 255f);
        public static readonly Color ActivatorPressBorderColor = new Color(51f / 255f, 51f / 255f, 51f / 255f);

        // Separator
        public const int ActivatorSeparatorFontSize = 11;
        
        public static readonly Color ActivatorSeparatorColor = new Color(56f / 255f, 56f / 255f, 56f / 255f);
        
        // Action text
        public const int ActivatorActionFontSize = 12;
        
        public static readonly Color ActivatorActionColor = new Color(170f / 255f, 170f / 255f, 170f / 255f);
        public static readonly Color ActivatorActionHighlightColor = new Color(141f / 255f, 190f / 255f, 69f / 255f);
        public static readonly Color ActivatorNoteColor = new Color(72f / 255f, 72f / 255f, 72f / 255f);

        // ===============================================================
        // Title bar
        // ===============================================================
        public const float TitleBarHeight = 28f;
        public const float TitleBarSeparatorHeight = 1f;
        public const string TitleBarGamepadIconPath = "SteamInput/Textures/gamepad_icon";
        public const float TitleBarActionGroupBorderThickness = 1f;
        
        // Colors
        public static readonly Color TitleBarBackgroundColor = new Color(46f / 255f, 46f / 255f, 46f / 255f);
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
        public const float MenuThickness = 1f;

        public static readonly Color MenuTitleColor = new Color(85f / 255f, 85f / 255f, 85f / 255f);
        public static readonly Color MenuBackgroundColor = new Color(30f / 255f, 30f / 255f, 30f / 255f);
        
        
        // ===============================================================
        // Rows for mouse
        // ===============================================================

        public const int MouseLineFontSize = 12;
        public const float MouseLinePaddingV = 4f;
        
        public static readonly Color MouseLineColor = new Color(85f / 255f, 85f / 255f, 85f / 255f);

        // ===============================================================
        // Combo box for contgroller config selection
        // ===============================================================
        
        // Combo itself
        public const int ComboFontSize = 12;
        public const int ComboCaretFontSize = 10;
        public const float ComboHeight = 22f;
        public const float ComboPaddingH = 8f;
        public const float ComboPaddingV = 6f;
        public const float ComboDropdownMaxHeight = 180f;
        
        public static readonly Color ComboTextColor = new Color(232f / 255f, 232f / 255f, 232f / 255f);
        public static readonly Color ComboCaretColor = new Color(136f / 255f, 136f / 255f, 136f / 255f);

        // Combo item
        public const int ComboItemTitleFontSize = 12;
        public const int ComboItemTypeFontSize = 10;
        public const float ComboItemPaddingH = 9f;
        public const float ComboItemPaddingV = 4f;
        
        public static readonly Color ComboItemTitleColor = new Color(221f / 255f, 221f / 255f, 221f / 255f);
        public static readonly Color ComboItemTitleSelectedColor = new Color(141f / 255f, 190f / 255f, 69f / 255f);
        public static readonly Color ComboItemTypeColor = new Color(102f / 255f, 102f / 255f, 102f / 255f); // #666
        public static readonly Color ComboItemNoneColor = new Color(136f / 255f, 136f / 255f, 136f / 255f);
        public static readonly Color ComboItemHoverColor = new Color(42f / 255f, 42f / 255f, 42f / 255f);






        
        






        
        public static readonly Color Label = new Color(204f / 255f, 204f / 255f, 204f / 255f);
        public static readonly Color Muted = new Color(136f / 255f, 136f / 255f, 136f / 255f);
        public static readonly Color Accent = new Color(141f / 255f, 190f / 255f, 69f / 255f);
        public static readonly Color Warn = new Color(0.95f, 0.82f, 0.23f);
        public static readonly Color Button = new Color(56f / 255f, 56f / 255f, 56f / 255f);
        
        public static readonly Color ButtonHover = new Color(72f / 255f, 72f / 255f, 72f / 255f);
        public static readonly Color MenuBox = new Color(30f / 255f, 30f / 255f, 30f / 255f);
    }
}
