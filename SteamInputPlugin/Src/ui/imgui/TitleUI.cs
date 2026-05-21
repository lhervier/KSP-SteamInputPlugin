using System;
using UnityEngine;
using com.github.lhervier.ksp;
using com.github.lhervier.ksp.ui.styles;

namespace com.github.lhervier.ksp.ui.imgui
{
    public class TitleUI
    {
        private const float BadgeControllerGap = 6f;
        private const float ControllerCloseGap = 6f;
        private const float BarContentHeight = 20f;

        private readonly CheatSheetViewModel viewModel;
        private readonly Action onClose;
        private readonly SteamInputIcon gamepadIcon;
        private readonly ZonesMenuUI zonesMenuUI;

        public TitleUI(
            CheatSheetViewModel viewModel, 
            Action onClose)
        {
            this.viewModel = viewModel;
            this.onClose = onClose;
            this.gamepadIcon = new SteamInputIcon(
                SteamInputPalette.GamepadIconPath, 
                SteamInputPalette.GamepadIconSize, 
                SteamInputPalette.GamepadIconSize
            );
            this.zonesMenuUI = new ZonesMenuUI();
        }

        public ZonesMenuUI ZonesMenu
        {
            get { return zonesMenuUI; }
        }

        public void DrawTitle()
        {
            GUILayout.BeginHorizontal(SteamInputStyles.HeaderBar, GUILayout.Height(SteamInputPalette.TitleBarHeight));

            DrawVerticallyCentered(
                () => gamepadIcon.Draw(), 
                SteamInputPalette.GamepadIconSize
            );

            GUILayout.Space(SteamInputPalette.IconTitleGap);

            DrawVerticallyCentered(
                () => GUILayout.Label(
                    ModLocalization.GetString("SteamInput_titleHelp"),
                    SteamInputStyles.Title
                )
            );

            GUILayout.FlexibleSpace();

            var badgeContent = new GUIContent(viewModel.GetActionGroupLabel());
            var badgeSize = SteamInputStyles.ActionSetBadge.CalcSize(badgeContent);
            DrawVerticallyCentered(
                () => GUILayout.Label(
                    badgeContent,
                    SteamInputStyles.ActionSetBadge,
                    GUILayout.Width(badgeSize.x),
                    GUILayout.Height(badgeSize.y)
                ),
                badgeSize.x
            );

            var controllerName = viewModel.GetGamepadLabel();
            if (!string.IsNullOrEmpty(controllerName))
            {
                GUILayout.Space(BadgeControllerGap);
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

            DrawVerticallyCentered(() => zonesMenuUI.DrawTitleBarButton(), 20f);

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
