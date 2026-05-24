using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;

namespace com.github.lhervier.ksp.ui.ugui.menu
{
    public class ZoneLabelBuilder
    {
        
        private CheatSheetViewModel _viewModel;
        
        public ZoneLabelBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public ZoneLabelController Create(string label)
        {
            var go = new GameObject("Label", typeof(RectTransform));
            ZoneLabelController controller = go.AddComponent<ZoneLabelController>();
            controller.Initialize(_viewModel);

            // Greedy on width: consumes the leftover space and pushes the arrows to the right
            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.flexibleWidth = 1f;

            var text = go.AddComponent<Text>();
            text.text = label;
            text.font = HighLogic.UISkin.font;
            text.fontSize = 12;
            text.color = SteamInputPalette.DefaultLabelColor;
            text.alignment = TextAnchor.MiddleLeft;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            controller.BindLabel(text);

            return controller;
        }

        public class ZoneLabelController : BaseSteamInputController
        {
            private Text _label;

            public void BindLabel(Text label)
            {
                _label = label;
            }

            public string GetLabel()
            {
                if( _label == null ) return string.Empty;
                return _label.text;
            }

            public void SetLabel(string label)
            {
                _label.text = label ?? string.Empty;
            }
        }
    }
}
