using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using UnityEngine.Events;

namespace com.github.lhervier.ksp.ui.ugui.titleBar
{
    public class RightColumnBuilder
    {
        private CheatSheetViewModel _viewModel;
        private OverlayBuilder _overlayBuilder;
        private ActionGroupLabelBuilder _actionGroupLabelBuilder;
        private GamepadLabelBuilder _gamepadLabelBuilder;
        private MenuButtonBuilder _menuButtonBuilder;
        private CloseButtonBuilder _closeButtonBuilder;

        public RightColumnBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._overlayBuilder = new OverlayBuilder(viewModel);
            this._actionGroupLabelBuilder = new ActionGroupLabelBuilder(viewModel);
            this._gamepadLabelBuilder = new GamepadLabelBuilder(viewModel);
            this._closeButtonBuilder = new CloseButtonBuilder(viewModel);
            this._menuButtonBuilder = new MenuButtonBuilder(viewModel);
        }

        public GameObject Create(UnityAction onMenuToggle)
        {
            var rightRowGo = new GameObject("SteamInput.TitleBar.RightColumn", typeof(RectTransform));

            // Horizontal layout containing the right-side placeholders, sized to their text content
            var rightRowLayout = rightRowGo.AddComponent<HorizontalLayoutGroup>();
            rightRowLayout.spacing = SteamInputPalette.DefaultSpacing;
            rightRowLayout.childAlignment = TextAnchor.MiddleLeft;
            rightRowLayout.childControlWidth = true;
            rightRowLayout.childControlHeight = true;
            rightRowLayout.childForceExpandWidth = false;
            rightRowLayout.childForceExpandHeight = false;

            var actionGroupGo = this._actionGroupLabelBuilder.Create();
            actionGroupGo.transform.SetParent(rightRowGo.transform, false);

            var controllerGo = this._gamepadLabelBuilder.Create();
            controllerGo.transform.SetParent(rightRowGo.transform, false);

            var menuGo = this._menuButtonBuilder.Create(onMenuToggle);
            menuGo.transform.SetParent(rightRowGo.transform, false);

            var closeGo = this._closeButtonBuilder.Create();
            closeGo.transform.SetParent(rightRowGo.transform, false);

            return rightRowGo;
        }
    }
}