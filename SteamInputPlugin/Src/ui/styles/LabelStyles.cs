using UnityEngine;

namespace com.github.lhervier.ksp.ui.styles
{
    /// <summary>Body labels (default, muted, accent, warn, error).</summary>
    public static class LabelStyles
    {
        public static GUIStyle Label { get; private set; }
        public static GUIStyle MutedLabel { get; private set; }
        public static GUIStyle AccentLabel { get; private set; }
        public static GUIStyle WarnLabel { get; private set; }
        public static GUIStyle ErrorLabel { get; private set; }

        internal static void Initialize()
        {
            Label = Create(SteamInputPalette.Label);
            MutedLabel = Create(SteamInputPalette.Muted);
            AccentLabel = Create(SteamInputPalette.Accent);
            WarnLabel = Create(SteamInputPalette.Warn);
            ErrorLabel = Create(Color.red);
            ErrorLabel.wordWrap = true;
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
