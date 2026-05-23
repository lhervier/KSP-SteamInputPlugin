using UnityEngine;

namespace com.github.lhervier.ksp.ui.styles
{
    internal sealed class SteamInputStyleTextures
    {
        public Texture2D Body;
        public Texture2D Header;
        public Texture2D Border;
        public Texture2D Button;
        public Texture2D FieldBackground;
        public Texture2D ButtonHover;
        public Texture2D MenuBox;
        public Texture2D ZonesMenuBackground;
        public Texture2D MenuSeparator;
        public Texture2D ZoneHeaderBackground;
        public Texture2D ZoneHeaderBottomLine;
        public Texture2D ZoneSeparatorLine;

        public static SteamInputStyleTextures Create()
        {
            return new SteamInputStyleTextures
            {
                Body = MakeTexture(SteamInputPalette.WindowBodyColor),
                Header = MakeTexture(SteamInputPalette.Header),
                Border = MakeTexture(SteamInputPalette.WindowBorderColor),
                Button = MakeTexture(SteamInputPalette.Button),
                FieldBackground = MakeTexture(SteamInputPalette.FieldBackground),
                ButtonHover = MakeTexture(SteamInputPalette.ButtonHover),
                MenuBox = MakeTexture(SteamInputPalette.MenuBox),
                ZonesMenuBackground = MakeTexture(SteamInputPalette.ZonesMenuBackground),
                MenuSeparator = MakeTexture(SteamInputPalette.DefaultSeparatorColor),
                ZoneHeaderBackground = MakeTexture(SteamInputPalette.ZoneHeaderBg),
                ZoneHeaderBottomLine = MakeTexture(SteamInputPalette.ZoneSeparator),
                ZoneSeparatorLine = MakeTexture(SteamInputPalette.ZoneSeparator),
            };
        }

        public static Texture2D MakeBorderTexture(Color fill, Color border, int thickness)
        {
            var size = 2 * thickness + 1;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var isBorder = x < thickness || x >= size - thickness
                                || y < thickness || y >= size - thickness;
                    tex.SetPixel(x, y, isBorder ? border : fill);
                }
            }
            tex.Apply();
            tex.filterMode = FilterMode.Point;
            tex.hideFlags = HideFlags.HideAndDontSave;
            return tex;
        }

        private static Texture2D MakeTexture(Color color)
        {
            var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            tex.filterMode = FilterMode.Point;
            tex.hideFlags = HideFlags.HideAndDontSave;
            return tex;
        }
    }
}
