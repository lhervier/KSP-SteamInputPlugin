using System;
using UnityEngine;

namespace com.github.lhervier.ksp.ui
{
    public class TitleUI
    {
        private const string GamepadIconPath = "SteamInput/Textures/gamepad_icon";
        private const int GamepadIconSize = 18;
        private const float IconTitleGap = 6f;

        private readonly Action onClose;
        private readonly SteamInputIcon gamepadIcon;

        public TitleUI(CheatSheetViewModel viewModel, Action onClose)
        {
            this.onClose = onClose;
            this.gamepadIcon = new SteamInputIcon(GamepadIconPath, GamepadIconSize, GamepadIconSize);
        }

        public void DrawTitle()
        {
            GUILayout.BeginHorizontal(SteamInputStyles.HeaderBar, GUILayout.Height(SteamInputStyles.TitleBarHeight));
            gamepadIcon.Draw();
            GUILayout.Space(IconTitleGap);
            GUILayout.Label("AIDE MANETTE", SteamInputStyles.Title, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("×", SteamInputStyles.CloseButton))
            {
                onClose();
            }
            GUILayout.EndHorizontal();
        }
    }
}
