using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.sprites;
using UnityEngine.Events;
using System;

namespace com.github.lhervier.ksp.ui.ugui.titleBar
{
    public class RightColumnBuilder
    {
        private CheatSheetViewModel _viewModel;
        private OverlayBuilder _overlayBuilder;
        private ActionGroupLabelBuilder _actionGroupLabelBuilder;
        private GamepadLabelBuilder _gamepadLabelBuilder;
        private ButtonBuilder _buttonBuilder;

        public RightColumnBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._overlayBuilder = new OverlayBuilder(viewModel);
            this._actionGroupLabelBuilder = new ActionGroupLabelBuilder(viewModel);
            this._gamepadLabelBuilder = new GamepadLabelBuilder(viewModel);
            this._buttonBuilder = new ButtonBuilder(viewModel);
        }

        public RightColumnController Create()
        {
            var rightRowGo = new GameObject("SteamInput.TitleBar.RightColumn", typeof(RectTransform));
            RightColumnController controller = rightRowGo.AddComponent<RightColumnController>();
            controller.Initialize(_viewModel);

            // Horizontal layout containing the right-side placeholders, sized to their text content
            var rightRowLayout = rightRowGo.AddComponent<HorizontalLayoutGroup>();
            rightRowLayout.spacing = SteamInputPalette.DefaultSpacing;
            rightRowLayout.childAlignment = TextAnchor.MiddleLeft;
            rightRowLayout.childControlWidth = true;
            rightRowLayout.childControlHeight = true;
            rightRowLayout.childForceExpandWidth = false;
            rightRowLayout.childForceExpandHeight = false;

            var actionGroupLabelController = this._actionGroupLabelBuilder.Create();
            actionGroupLabelController.transform.SetParent(rightRowGo.transform, false);

            var controllerGo = this._gamepadLabelBuilder.Create();
            controllerGo.transform.SetParent(rightRowGo.transform, false);

            var menuButtonController = this._buttonBuilder.Create(
                "SteamInput.TitleBar.RightColumn.MenuButton",
                "⋯",
                () => _viewModel.ToggleMenu(),
                true,
                SteamInputPalette.TitleBarButtonColor,
                SteamInputPalette.TitleBarButtonHoverColor
            );
            menuButtonController.transform.SetParent(rightRowGo.transform, false);

            var closeButtonController = this._buttonBuilder.Create(
                "SteamInput.TitleBar.RightColumn.Close",
                "×",
                this._viewModel.CloseWindow,
                true,
                SteamInputPalette.TitleBarButtonColor,
                SteamInputPalette.TitleBarButtonHoverColor
            );
            closeButtonController.transform.SetParent(rightRowGo.transform, false);

            return controller;
        }

        public class RightColumnController : BaseSteamInputController
        {
        }
    }
}