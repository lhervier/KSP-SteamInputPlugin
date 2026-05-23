using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.ui.styles;

namespace com.github.lhervier.ksp.ui.ugui.titleBar
{
    public class TitleBuilder
    {
        private CheatSheetViewModel _viewModel;
        public TitleBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public TitleController Create()
        {
            var labelGo = new GameObject("SteamInput.TitleBar.LeftColumn.Label", typeof(RectTransform));
            TitleController controller = labelGo.AddComponent<TitleController>();
            controller.Initialize(this._viewModel);
            
            var label = labelGo.AddComponent<Text>();
            label.text = ModLocalization.GetString("SteamInput_titleHelp").ToUpperInvariant();
            label.font = HighLogic.UISkin.font;
            label.fontSize = 12;
            label.fontStyle = FontStyle.Bold;
            label.color = SteamInputPalette.TitleBarLabelColor;
            label.alignment = TextAnchor.MiddleLeft;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;

            return controller;
        }

        public class TitleController : BaseSteamInputController
        {
        }
    }
}