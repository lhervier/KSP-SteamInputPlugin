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

        public static SteamInputStyleTextures Create()
        {
            return new SteamInputStyleTextures
            {
                Body = MakeTexture(SteamInputPalette.Body),
                Header = MakeTexture(SteamInputPalette.Header),
                Border = MakeTexture(SteamInputPalette.Border),
                Button = MakeTexture(SteamInputPalette.Button),
                FieldBackground = MakeTexture(SteamInputPalette.FieldBackground),
                ButtonHover = MakeTexture(SteamInputPalette.ButtonHover),
                MenuBox = MakeTexture(SteamInputPalette.MenuBox),
            };
        }

        public static Texture2D MakeBorderTexture(Color fill, Color border)
        {
            var tex = new Texture2D(3, 3, TextureFormat.RGBA32, false);
            for (var y = 0; y < 3; y++)
            {
                for (var x = 0; x < 3; x++)
                {
                    var isBorder = x == 0 || x == 2 || y == 0 || y == 2;
                    tex.SetPixel(x, y, isBorder ? border : fill);
                }
            }
            tex.Apply();
            tex.hideFlags = HideFlags.HideAndDontSave;
            return tex;
        }

        private static Texture2D MakeTexture(Color color)
        {
            var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            tex.hideFlags = HideFlags.HideAndDontSave;
            return tex;
        }
    }
}
