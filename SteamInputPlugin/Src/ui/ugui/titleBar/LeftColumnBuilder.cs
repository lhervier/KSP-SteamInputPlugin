using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using UnityEngine.Events;

namespace com.github.lhervier.ksp.ui.ugui.titleBar
{
    public class LeftColumnBuilder
    {
        private CheatSheetViewModel _viewModel;
        private GamepadIconBuilder _gamepadIconBuilder;
        private TitleBuilder _titleBuilder;

        public LeftColumnBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._gamepadIconBuilder = new GamepadIconBuilder(viewModel);
            this._titleBuilder = new TitleBuilder(viewModel);
        }

        public GameObject Create()
        {
            var leftRowGo = new GameObject("SteamInput.TitleBar.LeftColumn", typeof(RectTransform));

            // Greedy on width so it consumes the leftover space and pushes the right row against the right edge
            var leftRowLayoutElement = leftRowGo.AddComponent<LayoutElement>();
            leftRowLayoutElement.flexibleWidth = 1f;

            // Horizontal layout containing icon + label
            var leftRowLayout = leftRowGo.AddComponent<HorizontalLayoutGroup>();
            leftRowLayout.spacing = SteamInputPalette.DefaultSpacing;
            leftRowLayout.childAlignment = TextAnchor.MiddleLeft;
            leftRowLayout.childControlWidth = false;
            leftRowLayout.childControlHeight = false;
            leftRowLayout.childForceExpandWidth = false;
            leftRowLayout.childForceExpandHeight = false;

            var iconGo = this._gamepadIconBuilder.Create();
            iconGo.transform.SetParent(leftRowLayoutElement.transform, false);

            var labelGo = this._titleBuilder.Create();
            labelGo.transform.SetParent(leftRowLayoutElement.transform, false);

            return leftRowGo;
        }
    }
}