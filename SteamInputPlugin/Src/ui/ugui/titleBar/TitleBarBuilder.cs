using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.ugui.shared;
using com.github.lhervier.ksp.ugui.shared.styles;
using com.github.lhervier.ksp.ugui.shared.button;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.titleBar
{
    public class TitleBarBuilder : IUGUIBuilder<TitleBarController>
    {
        private CheatSheetViewModel _viewModel;
        private ActionGroupLabelBuilder _actionGroupLabelBuilder;
        private GamepadLabelBuilder _gamepadLabelBuilder;
        private ButtonBuilder _buttonBuilder;

        public TitleBarBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._actionGroupLabelBuilder = new ActionGroupLabelBuilder(viewModel);
            this._gamepadLabelBuilder = new GamepadLabelBuilder(viewModel);
            this._buttonBuilder = new ButtonBuilder();
        }

        public TitleBarController Create()
        {
            var rightRowGo = new GameObject("SteamInput.TitleBar.RightColumn", typeof(RectTransform));
            TitleBarController controller = rightRowGo.AddComponent<TitleBarController>();

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

            return controller;
        }
    }
}