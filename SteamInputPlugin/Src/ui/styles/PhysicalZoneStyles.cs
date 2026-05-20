using UnityEngine;

namespace com.github.lhervier.ksp.ui.styles
{
    /// <summary>Physical zone list (.kzone, .kzh, .kstate) from ksp_cheatsheet mockup.</summary>
    public static class PhysicalZoneStyles
    {
        public static GUIStyle ZoneListPanel { get; private set; }
        public static GUIStyle ZoneBody { get; private set; }
        public static GUIStyle ZoneHeaderBar { get; private set; }
        public static GUIStyle ZoneName { get; private set; }
        public static GUIStyle SectionNormal { get; private set; }
        public static GUIStyle SectionModeshift { get; private set; }
        public static GUIStyle ZoneSeparator { get; private set; }

        internal static void Initialize(SteamInputStyleTextures textures)
        {
            ZoneListPanel = new GUIStyle
            {
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0)
            };
            ZoneListPanel.normal.background = textures.Body;

            ZoneBody = new GUIStyle
            {
                padding = new RectOffset(0, 0, 3, 5),
                margin = new RectOffset(0, 0, 0, 0)
            };
            ZoneBody.normal.background = textures.Body;

            ZoneHeaderBar = new GUIStyle
            {
                fixedHeight = SteamInputPalette.ZoneHeaderHeight,
                padding = new RectOffset(8, 8, 4, 4),
                margin = new RectOffset(0, 0, 0, 0),
                border = new RectOffset(0, 0, 0, 1)
            };
            ZoneHeaderBar.normal.background = SteamInputStyleTextures.MakeBorderTexture(
                SteamInputPalette.ZoneHeaderBg,
                SteamInputPalette.ZoneSeparator);

            ZoneName = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                clipping = TextClipping.Clip,
                padding = new RectOffset(0, 0, 0, 0)
            };
            ZoneName.normal.textColor = SteamInputPalette.ZoneName;

            SectionNormal = CreateSectionHeader(SteamInputPalette.SectionNormal);
            SectionModeshift = CreateSectionHeader(SteamInputPalette.SectionModeshift);

            ZoneSeparator = new GUIStyle
            {
                fixedHeight = 1,
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(0, 0, 0, 0)
            };
            ZoneSeparator.normal.background = SteamInputStyleTextures.MakeBorderTexture(
                SteamInputPalette.ZoneSeparator,
                SteamInputPalette.ZoneSeparator);
        }

        private static GUIStyle CreateSectionHeader(Color textColor)
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 10,
                clipping = TextClipping.Clip,
                padding = new RectOffset(8, 8, 3, 1),
                margin = new RectOffset(0, 0, 0, 0)
            };
            style.normal.textColor = textColor;
            return style;
        }
    }
}
