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

        public static GUIStyle Label => LabelStyles.Label;
        public static GUIStyle MutedLabel => LabelStyles.MutedLabel;
        public static GUIStyle AccentLabel => LabelStyles.AccentLabel;
        public static GUIStyle WarnLabel => LabelStyles.WarnLabel;
        public static GUIStyle ErrorLabel => LabelStyles.ErrorLabel;

        public static GUIStyle TextField => FormControlStyles.TextField;
        public static GUIStyle Toggle => FormControlStyles.Toggle;
        public static GUIStyle MenuButton => FormControlStyles.MenuButton;
        public static GUIStyle MenuBox => FormControlStyles.MenuBox;

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
            _ready = true;
        }
    }
}
