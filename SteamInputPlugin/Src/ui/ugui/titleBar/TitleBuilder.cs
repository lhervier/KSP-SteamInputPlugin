using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using UnityEngine.Events;

namespace com.github.lhervier.ksp.ui.ugui.titleBar
{
    public class TitleBuilder
    {
        private CheatSheetViewModel _viewModel;
        public TitleBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public GameObject Create()
        {
            var labelGo = new GameObject("SteamInput.TitleBar.LeftColumn.Label", typeof(RectTransform));
            
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

            return labelGo;
        }
    }
}