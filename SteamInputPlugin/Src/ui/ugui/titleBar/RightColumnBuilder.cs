using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.ugui.shared.styles;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.titleBar
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
            rightRowLayout.spacing = DefaultPalette.Spacing;
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
                PopupPalette.TitleBarButtonColor,
                PopupPalette.TitleBarButtonHoverColor
            );
            menuButtonController.transform.SetParent(rightRowGo.transform, false);

            var closeButtonController = this._buttonBuilder.Create(
                "SteamInput.TitleBar.RightColumn.Close",
                "×",
                this._viewModel.CloseWindow,
                true,
                PopupPalette.TitleBarButtonColor,
                PopupPalette.TitleBarButtonHoverColor
            );
            closeButtonController.transform.SetParent(rightRowGo.transform, false);

            return controller;
        }

        public class RightColumnController : BaseSteamInputController
        {
        }
    }
}