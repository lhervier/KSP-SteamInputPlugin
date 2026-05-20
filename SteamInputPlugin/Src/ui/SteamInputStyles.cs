using UnityEngine;

namespace com.github.lhervier.ksp.ui
{
    /// <summary>
    /// IMGUI styles for Steam Input windows (palette from ksp_cheatsheet_xbox.html).
    /// </summary>
    public static class SteamInputStyles
    {
        public const float WindowWidth = 400f;
        public const float TitleBarHeight = 28f;

        // Mockup palette
        private static readonly Color ColBody = new Color(20f / 255f, 20f / 255f, 20f / 255f);
        private static readonly Color ColHeader = new Color(46f / 255f, 46f / 255f, 46f / 255f);
        private static readonly Color ColBorder = new Color(85f / 255f, 85f / 255f, 85f / 255f);
        private static readonly Color ColTitleText = new Color(232f / 255f, 232f / 255f, 232f / 255f);
        private static readonly Color ColLabel = new Color(204f / 255f, 204f / 255f, 204f / 255f);
        private static readonly Color ColMuted = new Color(136f / 255f, 136f / 255f, 136f / 255f);
        private static readonly Color ColControllerName = new Color(85f / 255f, 85f / 255f, 85f / 255f);
        private static readonly Color ColAccent = new Color(141f / 255f, 190f / 255f, 69f / 255f);
        private static readonly Color ColBadgeBorder = new Color(74f / 255f, 110f / 255f, 32f / 255f);
        private static readonly Color ColWarn = new Color(0.95f, 0.82f, 0.23f);
        private static readonly Color ColBtn = new Color(56f / 255f, 56f / 255f, 56f / 255f);
        private static readonly Color ColBtnText = new Color(187f / 255f, 187f / 255f, 187f / 255f);
        private static readonly Color ColFieldBg = new Color(42f / 255f, 42f / 255f, 42f / 255f);
        private static readonly Color ColBtnHover = new Color(72f / 255f, 72f / 255f, 72f / 255f);
        private static readonly Color ColMenuBox = new Color(30f / 255f, 30f / 255f, 30f / 255f);

        private static bool _ready;

        public static GUIStyle Window { get; private set; }
        public static GUIStyle Body { get; private set; }
        public static GUIStyle HeaderBar { get; private set; }
        public static GUIStyle Title { get; private set; }
        public static GUIStyle ActionSetBadge { get; private set; }
        public static GUIStyle ControllerName { get; private set; }
        public static GUIStyle Label { get; private set; }
        public static GUIStyle MutedLabel { get; private set; }
        public static GUIStyle AccentLabel { get; private set; }
        public static GUIStyle WarnLabel { get; private set; }
        public static GUIStyle ErrorLabel { get; private set; }
        public static GUIStyle CloseButton { get; private set; }
        public static GUIStyle TextField { get; private set; }
        public static GUIStyle Toggle { get; private set; }
        public static GUIStyle MenuButton { get; private set; }
        public static GUIStyle MenuBox { get; private set; }

        /// <summary>Must be called from OnGUI only (uses GUI.skin).</summary>
        public static void EnsureInitialized()
        {
            if (_ready)
            {
                return;
            }

            var texBody = MakeTexture(ColBody);
            var texHeader = MakeTexture(ColHeader);
            var texBorder = MakeTexture(ColBorder);
            var texBtn = MakeTexture(ColBtn);
            var texField = MakeTexture(ColFieldBg);
            var texBtnHover = MakeTexture(ColBtnHover);
            var texMenuBox = MakeTexture(ColMenuBox);

            Window = new GUIStyle(GUI.skin.window)
            {
                padding = new RectOffset(1, 1, 1, 1),
                border = new RectOffset(1, 1, 1, 1)
            };
            Window.normal.background = texBorder;
            Window.onNormal.background = texBorder;
            Window.focused.background = texBorder;
            Window.onFocused.background = texBorder;

            Body = new GUIStyle
            {
                padding = new RectOffset(8, 8, 6, 8)
            };
            Body.normal.background = texBody;

            HeaderBar = new GUIStyle
            {
                padding = new RectOffset(8, 6, 4, 4),
                fixedHeight = TitleBarHeight
            };
            HeaderBar.normal.background = texHeader;

            Title = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                clipping = TextClipping.Clip
            };
            Title.normal.textColor = ColTitleText;

            ActionSetBadge = new GUIStyle(GUI.skin.label)
            {
                fontSize = 10,
                padding = new RectOffset(5, 5, 2, 2),
                border = new RectOffset(1, 1, 1, 1),
                clipping = TextClipping.Clip,
                wordWrap = false,
                stretchWidth = false
            };
            ActionSetBadge.normal.textColor = ColAccent;
            ActionSetBadge.normal.background = MakeBorderTexture(Color.clear, ColBadgeBorder);

            ControllerName = new GUIStyle(GUI.skin.label)
            {
                fontSize = 10,
                clipping = TextClipping.Clip,
                wordWrap = false,
                stretchWidth = false
            };
            ControllerName.normal.textColor = ColControllerName;

            Label = CreateLabelStyle(ColLabel);
            MutedLabel = CreateLabelStyle(ColMuted);
            AccentLabel = CreateLabelStyle(ColAccent);
            WarnLabel = CreateLabelStyle(ColWarn);
            ErrorLabel = CreateLabelStyle(Color.red);
            ErrorLabel.wordWrap = true;

            CloseButton = new GUIStyle(GUI.skin.button)
            {
                fontSize = 13,
                fixedWidth = 20,
                fixedHeight = 20,
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0)
            };
            CloseButton.normal.background = texBtn;
            CloseButton.normal.textColor = ColBtnText;
            CloseButton.hover.background = texBtnHover;
            CloseButton.hover.textColor = Color.white;
            CloseButton.active.background = texBtn;
            CloseButton.active.textColor = ColBtnText;

            TextField = new GUIStyle(GUI.skin.textField)
            {
                fontSize = 12,
                padding = new RectOffset(4, 4, 3, 3)
            };
            TextField.normal.textColor = ColTitleText;
            TextField.normal.background = texField;
            TextField.focused.background = texField;
            TextField.focused.textColor = ColTitleText;

            Toggle = new GUIStyle(GUI.skin.toggle)
            {
                fontSize = 12,
                padding = new RectOffset(20, 0, 2, 0)
            };
            Toggle.normal.textColor = ColLabel;
            Toggle.onNormal.textColor = ColLabel;
            Toggle.hover.textColor = ColTitleText;

            MenuButton = new GUIStyle(CloseButton)
            {
                fixedWidth = 100
            };

            MenuBox = new GUIStyle
            {
                padding = new RectOffset(4, 4, 4, 4)
            };
            MenuBox.normal.background = texMenuBox;

            _ready = true;
        }

        private static GUIStyle CreateLabelStyle(Color textColor)
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                wordWrap = false
            };
            style.normal.textColor = textColor;
            return style;
        }

        private static Texture2D MakeTexture(Color color)
        {
            var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            tex.hideFlags = HideFlags.HideAndDontSave;
            return tex;
        }

        private static Texture2D MakeBorderTexture(Color fill, Color border)
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
    }
}
