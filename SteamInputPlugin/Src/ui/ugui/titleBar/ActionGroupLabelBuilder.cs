using UnityEngine;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.badge;
using com.github.lhervier.ksp.shared.ugui.styles;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.titleBar
{
    public class ActionGroupLabelBuilder : IUGUIBuilder<ActionGroupLabelController>
    {
        // ========================================
        // Builder parameters
        // ========================================

        private CheatSheetViewModel _viewModel;
        
        public ActionGroupLabelBuilder WithViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        // ================================
        // Build
        // ================================
        
        public ActionGroupLabelController Build()
        {
            // Transparent-fill, green-border accent badge. The content-size fitter makes it keep hugging
            // its label width even when the title bar row is squeezed below its preferred size.
            BadgeController badge = new BadgeBuilder()
                .WithObjectName("SteamInput.TitleBar.RightColumn.ActionGroup")
                .WithColors(DefaultPalette.AccentColor, Color.clear, SteamInputPalette.TitleBarActionGroupBorderColor)
                .WithBorderThickness((int) PopupPalette.TitleBarActionGroupBorderThickness)
                .WithFontSize(SteamInputPalette.TitleBarActionGroupFontSize)
                .WithPadding(SteamInputPalette.TitleBarActionGroupPaddingH, SteamInputPalette.TitleBarActionGroupPaddingV)
                .WithContentSizeFitter()
                .Build();

            return badge.gameObject
                .AddComponent<ActionGroupLabelController>()
                .WithViewModel(this._viewModel)
                .WithBadge(badge);
        }
    }
}