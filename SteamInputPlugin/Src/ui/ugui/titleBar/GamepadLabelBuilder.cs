using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using UnityEngine.Events;
using com.github.lhervier.ksp.shared.ugui;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.titleBar
{
    public class GamepadLabelBuilder : IUGUIBuilder<GamepadLabelController>
    {
        private CheatSheetViewModel _viewModel;
        
        public GamepadLabelBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public GamepadLabelController Build()
        {
            var go = new GameObject("SteamInput.TitleBar.RightColumn.GamepadName", typeof(RectTransform));
            
            var label = go.AddComponent<Text>();
            label.text = "<gamepad>";
            label.font = HighLogic.UISkin.font;
            label.fontSize = 10;
            label.color = SteamInputPalette.TitleBarControllerNameColor;
            label.alignment = TextAnchor.MiddleLeft;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;
            return go.
                AddComponent<GamepadLabelController>()
                .ViewModel(this._viewModel)
                .Label(label);
        }
    }
}