using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.ugui.shared.styles;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.titleBar
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

        public LeftColumnController Create()
        {
            var leftColumnGo = new GameObject("SteamInput.TitleBar.LeftColumn", typeof(RectTransform));
            LeftColumnController controller = leftColumnGo.AddComponent<LeftColumnController>();
            controller.Initialize(this._viewModel);
            
            // Greedy on width so it consumes the leftover space and pushes the right row against the right edge
            var leftColumnLayoutElement = leftColumnGo.AddComponent<LayoutElement>();
            leftColumnLayoutElement.flexibleWidth = 1f;

            // Horizontal layout containing icon + label
            var leftColumnLayout = leftColumnGo.AddComponent<HorizontalLayoutGroup>();
            leftColumnLayout.spacing = DefaultPalette.Spacing;
            leftColumnLayout.childAlignment = TextAnchor.MiddleLeft;
            leftColumnLayout.childControlWidth = false;
            leftColumnLayout.childControlHeight = false;
            leftColumnLayout.childForceExpandWidth = false;
            leftColumnLayout.childForceExpandHeight = false;

            var iconGo = this._gamepadIconBuilder.Create();
            iconGo.transform.SetParent(leftColumnLayoutElement.transform, false);

            var labelGo = this._titleBuilder.Create();
            labelGo.transform.SetParent(leftColumnLayoutElement.transform, false);

            return controller;
        }

        public class LeftColumnController : BaseSteamInputController
        {
        }
    }
}