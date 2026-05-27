using UnityEngine;

namespace com.github.lhervier.ksp.ui.styles
{
    /// <summary>
    /// Entry point for Steam Input IMGUI styles. Must call <see cref="EnsureInitialized"/> from OnGUI.
    /// </summary>
    public static class SteamInputStyles
    {
        private static bool _ready;

        public static GUIStyle Window => WindowShellStyles.Window;
        public static GUIStyle Body => WindowShellStyles.Body;
        
        public static GUIStyle Label => LabelStyles.Label;
        public static GUIStyle AccentLabel => LabelStyles.AccentLabel;
        public static GUIStyle WarnLabel => LabelStyles.WarnLabel;
        
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
