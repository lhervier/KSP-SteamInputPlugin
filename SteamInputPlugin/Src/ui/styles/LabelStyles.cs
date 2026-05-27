using UnityEngine;

namespace com.github.lhervier.ksp.ui.styles
{
    /// <summary>Body labels (default, muted, accent, warn, error).</summary>
    public static class LabelStyles
    {
        public static GUIStyle Label { get; private set; }
        public static GUIStyle AccentLabel { get; private set; }
        public static GUIStyle WarnLabel { get; private set; }
        
        internal static void Initialize()
        {
            Label = Create(SteamInputPalette.Label);
            AccentLabel = Create(SteamInputPalette.Accent);
            WarnLabel = Create(SteamInputPalette.Warn);
        }

        private static GUIStyle Create(Color textColor)
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                wordWrap = false
            };
            style.normal.textColor = textColor;
            return style;
        }
    }
}
