using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using UnityEngine.Events;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.styles;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.titleBar
{
    public class GamepadLabelBuilder : IUGUIBuilder<GamepadLabelController>
    {
        // ========================================
        // Builder parameters
        // ========================================

        private CheatSheetViewModel _viewModel;
        
        public GamepadLabelBuilder ViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        // ===================================
        // Build
        // ===================================

        public GamepadLabelController Build()
        {
            var go = new GameObject("SteamInput.TitleBar.RightColumn.GamepadName", typeof(RectTransform));
            
            var label = go.AddComponent<TextMeshProUGUI>();
            label.text = "<gamepad>";
            label.font = DefaultPalette.Font;
            label.fontSize = SteamInputPalette.TitleBarControllerNameFontSize;
            label.color = SteamInputPalette.TitleBarControllerNameColor;
            label.alignment = TextAlignmentOptions.Left;
            label.enableWordWrapping = false;
            // The name is the only squeezable element of the title bar (its min width is 0): when
            // the row runs out of space, truncate it with "…" instead of drawing under the buttons.
            label.overflowMode = TextOverflowModes.Ellipsis;
            label.raycastTarget = false;
            return go.
                AddComponent<GamepadLabelController>()
                .ViewModel(this._viewModel)
                .Label(label);
        }
    }
}