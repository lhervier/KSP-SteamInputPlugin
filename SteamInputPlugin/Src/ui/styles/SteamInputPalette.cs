using UnityEngine;

namespace com.github.lhervier.ksp.steaminput.ui.styles
{
    public static class SteamInputPalette
    {
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
        public const string TitleBarGamepadIconPath = "SteamInput/Textures/gamepad_icon";
        
        // Colors
        public static readonly Color TitleBarLabelColor = new Color(232f / 255f, 232f / 255f, 232f / 255f);
        public static readonly Color TitleBarActionGroupBorderColor = new Color(74f / 255f, 110f / 255f, 32f / 255f);
        public static readonly Color TitleBarControllerNameColor = new Color(85f / 255f, 85f / 255f, 85f / 255f);
        
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
        public const float MenuIconSize = 22f;
        public const string MenuSettingsIconPath = "SteamInput/Textures/settings_icon";

        public static readonly Color MenuTitleColor = new Color(85f / 255f, 85f / 255f, 85f / 255f);
        public static readonly Color MenuBackgroundColor = new Color(30f / 255f, 30f / 255f, 30f / 255f);
        public static readonly Color MenuIconColor = new Color(136f / 255f, 136f / 255f, 136f / 255f);        // #888, like the mockup ⚙
        public static readonly Color MenuIconHoverColor = new Color(141f / 255f, 190f / 255f, 69f / 255f);   // accent green on hover
        
        
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

        // ===============================================================
        // Settings screen (head + logging / diagnostic sections)
        // ===============================================================

        // Head (back button + title), mockup .kset-head
        public const int SettingsHeadPaddingH = 8;
        public const int SettingsHeadPaddingV = 5;
        public const float SettingsHeadSpacing = 7f;
        public const int SettingsHeadTitleFontSize = 11;

        public static readonly Color SettingsHeadBgColor = new Color(26f / 255f, 26f / 255f, 26f / 255f);   // #1a1a1a
        public static readonly Color SettingsHeadTitleColor = new Color(221f / 255f, 221f / 255f, 221f / 255f); // #ddd

        // Section, mockup .kset-section / .kset-label
        public const int SettingsSectionPaddingH = 10;
        public const int SettingsSectionPaddingTop = 10;
        public const int SettingsSectionPaddingBottom = 12;
        public const float SettingsSectionSpacing = 7f;
        public const int SettingsLabelFontSize = 11;

        public static readonly Color SettingsLabelColor = new Color(141f / 255f, 190f / 255f, 69f / 255f);   // accent
        public static readonly Color SettingsSeparatorColor = new Color(34f / 255f, 34f / 255f, 34f / 255f); // #222

        // Hint box, mockup .kset-hint
        public const int SettingsHintFontSize = 11;
        public const int SettingsHintPaddingH = 9;
        public const int SettingsHintPaddingV = 7;

        public static readonly Color SettingsHintBgColor = new Color(24f / 255f, 24f / 255f, 24f / 255f);     // #181818
        public static readonly Color SettingsHintBorderColor = new Color(42f / 255f, 42f / 255f, 42f / 255f); // #2a2a2a
        public static readonly Color SettingsHintTextColor = new Color(153f / 255f, 153f / 255f, 153f / 255f);// #999

        // Diagnostic note box, mockup .kset-note
        public const int SettingsNotePaddingH = 9;
        public const int SettingsNotePaddingV = 6;

        public static readonly Color SettingsNoteBgColor = new Color(176f / 255f, 115f / 255f, 24f / 255f, 0.07f);
        public static readonly Color SettingsNoteBorderColor = new Color(74f / 255f, 58f / 255f, 24f / 255f); // #4a3a18
        public static readonly Color SettingsNoteTextColor = new Color(169f / 255f, 138f / 255f, 74f / 255f); // #a98a4a

        // Key/value row + badge, mockup .kset-kv / .kset-badge
        public const float SettingsKvHeight = 22f;
        public const int SettingsKvFontSize = 12;
        public const int SettingsBadgeFontSize = 10;
        public const int SettingsBadgePaddingH = 6;

        public static readonly Color SettingsKvKeyColor = new Color(187f / 255f, 187f / 255f, 187f / 255f);   // #bbb
        public static readonly Color SettingsBadgeOkTextColor = new Color(141f / 255f, 190f / 255f, 69f / 255f);
        public static readonly Color SettingsBadgeOkBorderColor = new Color(74f / 255f, 110f / 255f, 32f / 255f);
        public static readonly Color SettingsBadgeOkBgColor = new Color(141f / 255f, 190f / 255f, 69f / 255f, 0.1f);
        public static readonly Color SettingsBadgeNoTextColor = new Color(192f / 255f, 89f / 255f, 79f / 255f);
        public static readonly Color SettingsBadgeNoBorderColor = new Color(110f / 255f, 42f / 255f, 32f / 255f);
        public static readonly Color SettingsBadgeNoBgColor = new Color(192f / 255f, 89f / 255f, 79f / 255f, 0.1f);

        // Sub-label + activated contexts box, mockup .kset-sub / .kset-ctx
        public const int SettingsSubFontSize = 10;
        public const int SettingsContextFontSize = 11;
        public const int SettingsContextPaddingH = 9;
        public const int SettingsContextPaddingV = 4;

        public static readonly Color SettingsSubColor = new Color(119f / 255f, 119f / 255f, 119f / 255f);     // #777
        public static readonly Color SettingsContextBgColor = new Color(13f / 255f, 13f / 255f, 13f / 255f);  // #0d0d0d
        public static readonly Color SettingsContextBorderColor = new Color(34f / 255f, 34f / 255f, 34f / 255f);
        public static readonly Color SettingsContextRowColor = new Color(169f / 255f, 138f / 255f, 74f / 255f);
        public static readonly Color SettingsContextEmptyColor = new Color(102f / 255f, 102f / 255f, 102f / 255f); // #666
        
        // Log level rotating button (replaces the mockup's combobox)
        public const int SettingsLogLevelFontSize = 12;
        public const float SettingsLogLevelHeight = 22f;
        public const float SettingsLogLevelPaddingH = 8f;
        public const int SettingsLogLevelCycleFontSize = 11;

        public static readonly Color SettingsLogLevelTextColor = new Color(232f / 255f, 232f / 255f, 232f / 255f);
        public static readonly Color SettingsLogLevelCycleColor = new Color(136f / 255f, 136f / 255f, 136f / 255f);
        public static readonly Color SettingsLogLevelBgColor = new Color(42f / 255f, 42f / 255f, 42f / 255f);
        public static readonly Color SettingsLogLevelHoverColor = new Color(51f / 255f, 51f / 255f, 51f / 255f);   // #333

        // ===============================================================
        // Empty placeholder (shown when no config is selected)
        // ===============================================================

        // .kempty: padded container with the title above and the wrapped body text below.
        public const int EmptyPaddingH = 12;
        public const int EmptyPaddingV = 14;
        public const float EmptyTitleSpacing = 6f;
        public const int EmptyTitleFontSize = 12;
        public const int EmptyBodyFontSize = 12;

        public static readonly Color EmptyTitleColor = new Color(170f / 255f, 170f / 255f, 170f / 255f); // #aaa
        public static readonly Color EmptyBodyColor = new Color(119f / 255f, 119f / 255f, 119f / 255f);  // #777
        public static readonly Color EmptyHighlightColor = new Color(141f / 255f, 190f / 255f, 69f / 255f); // accent
    }
}
