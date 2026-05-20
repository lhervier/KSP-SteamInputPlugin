using System;
using UnityEngine;
using com.github.lhervier.ksp;

namespace com.github.lhervier.ksp.ui
{
    public class TitleUI
    {
        private const string GamepadIconPath = "SteamInput/Textures/gamepad_icon";
        private const int GamepadIconSize = 18;
        private const float IconTitleGap = 6f;
        private const float BadgeControllerGap = 6f;
        private const float ControllerCloseGap = 6f;
        private const float BarContentHeight = 20f;

        private readonly ActionGroupDaemon actionGroupDaemon;
        private readonly CheatSheetViewModel viewModel;
        private readonly Action onClose;
        private readonly SteamInputIcon gamepadIcon;

        public TitleUI(
            CheatSheetViewModel viewModel, 
            ActionGroupDaemon actionGroupDaemon,
            Action onClose)
        {
            this.viewModel = viewModel;
            this.actionGroupDaemon = actionGroupDaemon; 
            this.onClose = onClose;
            this.gamepadIcon = new SteamInputIcon(GamepadIconPath, GamepadIconSize, GamepadIconSize);
        }

        public void DrawTitle()
        {
            GUILayout.BeginHorizontal(SteamInputStyles.HeaderBar, GUILayout.Height(SteamInputStyles.TitleBarHeight));

            DrawVerticallyCentered(() => gamepadIcon.Draw(), GamepadIconSize);

            GUILayout.Space(IconTitleGap);

            DrawVerticallyCentered(
                () => GUILayout.Label(
                    ModLocalization.GetString("SteamInput_titleHelp"),
                    SteamInputStyles.Title,
                    GUILayout.ExpandWidth(true)),
                expandWidth: true);

            var badgeContent = new GUIContent(viewModel.GetActionGroupLabel(actionGroupDaemon.GetCurrentActionGroup()));
            var badgeSize = SteamInputStyles.ActionSetBadge.CalcSize(badgeContent);
            DrawVerticallyCentered(
                () => GUILayout.Label(
                    badgeContent,
                    SteamInputStyles.ActionSetBadge,
                    GUILayout.Width(badgeSize.x),
                    GUILayout.Height(badgeSize.y)),
                badgeSize.x);

            var controllerName = viewModel.GetGamepadLabel();
            if (!string.IsNullOrEmpty(controllerName))
            {
                GUILayout.Space(BadgeControllerGap);
                GUILayout.FlexibleSpace();
                var controllerContent = new GUIContent(controllerName);
                var controllerSize = SteamInputStyles.ControllerName.CalcSize(controllerContent);
                DrawVerticallyCentered(
                    () => GUILayout.Label(
                        controllerContent,
                        SteamInputStyles.ControllerName,
                        GUILayout.Width(controllerSize.x),
                        GUILayout.Height(controllerSize.y)),
                    controllerSize.x);
            }

            GUILayout.Space(ControllerCloseGap);

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
