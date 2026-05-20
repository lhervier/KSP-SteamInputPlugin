using UnityEngine;

namespace com.github.lhervier.ksp.ui.styles
{
    /// <summary>
    /// Entry point for Steam Input IMGUI styles. Must call <see cref="EnsureInitialized"/> from OnGUI.
    /// </summary>
    public static class SteamInputStyles
    {
        private static bool _ready;

        public static float WindowWidth => SteamInputPalette.WindowWidth;
        public static float TitleBarHeight => SteamInputPalette.TitleBarHeight;

        public static GUIStyle Window => WindowShellStyles.Window;
        public static GUIStyle Body => WindowShellStyles.Body;
        public static GUIStyle HeaderBar => WindowShellStyles.HeaderBar;

        public static GUIStyle Title => TitleBarStyles.Title;
        public static GUIStyle ActionSetBadge => TitleBarStyles.ActionSetBadge;
        public static GUIStyle ControllerName => TitleBarStyles.ControllerName;
        public static GUIStyle CloseButton => TitleBarStyles.CloseButton;
        public static GUIStyle TitleBarMenuButton => TitleBarStyles.MenuButton;

        public static GUIStyle ZonesMenuPanel => ZonesMenuStyles.Panel;
        public static GUIStyle ZonesMenuTitle => ZonesMenuStyles.Title;
        public static GUIStyle ZonesMenuSeparator => ZonesMenuStyles.Separator;

        public static GUIStyle Label => LabelStyles.Label;
        public static GUIStyle MutedLabel => LabelStyles.MutedLabel;
        public static GUIStyle AccentLabel => LabelStyles.AccentLabel;
        public static GUIStyle WarnLabel => LabelStyles.WarnLabel;
        public static GUIStyle ErrorLabel => LabelStyles.ErrorLabel;

        public static GUIStyle TextField => FormControlStyles.TextField;
        public static GUIStyle Toggle => FormControlStyles.Toggle;
        public static GUIStyle MenuButton => FormControlStyles.MenuButton;
        public static GUIStyle MenuBox => FormControlStyles.MenuBox;

        public static GUIStyle ZoneListPanel => PhysicalZoneStyles.ZoneListPanel;
        public static GUIStyle ZoneBody => PhysicalZoneStyles.ZoneBody;
        public static GUIStyle ZoneHeaderBar => PhysicalZoneStyles.ZoneHeaderBar;
        public static GUIStyle ZoneName => PhysicalZoneStyles.ZoneName;
        public static GUIStyle SectionNormal => PhysicalZoneStyles.SectionNormal;
        public static GUIStyle SectionModeshift => PhysicalZoneStyles.SectionModeshift;
        public static GUIStyle ZoneSeparator => PhysicalZoneStyles.ZoneSeparator;

        public static void EnsureInitialized()
        {
            if (_ready)
            {
                return;
            }

            var textures = SteamInputStyleTextures.Create();
            WindowShellStyles.Initialize(textures);
            TitleBarStyles.Initialize(textures);
            LabelStyles.Initialize();
            FormControlStyles.Initialize(textures);
            PhysicalZoneStyles.Initialize(textures);
            ZonesMenuStyles.Initialize(textures);
            _ready = true;
        }
    }
}
