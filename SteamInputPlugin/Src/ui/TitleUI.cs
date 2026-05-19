using System;
using UnityEngine;

namespace com.github.lhervier.ksp.ui
{
    public class TitleUI
    {
        private const string GamepadIconPath = "SteamInput/Textures/gamepad_icon";
        private const int GamepadIconSize = 18;
        private const float IconTitleGap = 6f;
        private const float BadgeCloseGap = 6f;
        private const float BarContentHeight = 20f;

        private readonly CheatSheetViewModel viewModel;
        private readonly Action onClose;
        private readonly SteamInputIcon gamepadIcon;

        public TitleUI(CheatSheetViewModel viewModel, Action onClose)
        {
            this.viewModel = viewModel;
            this.onClose = onClose;
            this.gamepadIcon = new SteamInputIcon(GamepadIconPath, GamepadIconSize, GamepadIconSize);
        }

        public void DrawTitle()
        {
            GUILayout.BeginHorizontal(SteamInputStyles.HeaderBar, GUILayout.Height(SteamInputStyles.TitleBarHeight));

            DrawVerticallyCentered(() => gamepadIcon.Draw(), GamepadIconSize);

            GUILayout.Space(IconTitleGap);

            DrawVerticallyCentered(
                () => GUILayout.Label("AIDE MANETTE", SteamInputStyles.Title, GUILayout.ExpandWidth(true)),
                expandWidth: true);

            var badgeContent = new GUIContent(viewModel.GetActionSetTitle().ToUpperInvariant());
            var badgeSize = SteamInputStyles.ActionSetBadge.CalcSize(badgeContent);
            DrawVerticallyCentered(
                () => GUILayout.Label(
                    badgeContent,
                    SteamInputStyles.ActionSetBadge,
                    GUILayout.Width(badgeSize.x),
                    GUILayout.Height(badgeSize.y)),
                badgeSize.x);

            GUILayout.Space(BadgeCloseGap);

            if (GUILayout.Button("×", SteamInputStyles.CloseButton))
            {
                onClose();
            }

            GUILayout.EndHorizontal();
        }

        private static void DrawVerticallyCentered(Action draw, float width = 0f, bool expandWidth = false)
        {
            if (expandWidth)
            {
                GUILayout.BeginVertical(GUILayout.Height(BarContentHeight), GUILayout.ExpandWidth(true));
            }
            else if (width > 0f)
            {
                GUILayout.BeginVertical(GUILayout.Width(width), GUILayout.Height(BarContentHeight));
            }
            else
            {
                GUILayout.BeginVertical(GUILayout.Height(BarContentHeight));
            }

            GUILayout.FlexibleSpace();
            draw();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }
    }
}
