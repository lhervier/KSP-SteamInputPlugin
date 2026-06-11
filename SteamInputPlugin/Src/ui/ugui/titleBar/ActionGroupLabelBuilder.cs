using UnityEngine;
using UnityEngine.UI;
using TMPro;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui.styles;
using com.github.lhervier.ksp.shared.ugui;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.titleBar
{
    public class ActionGroupLabelBuilder : IUGUIBuilder<ActionGroupLabelController>
    {
        // ========================================
        // Builder parameters
        // ========================================

        private CheatSheetViewModel _viewModel;
        
        public ActionGroupLabelBuilder ViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        // ================================
        // Build
        // ================================
        
        public ActionGroupLabelController Build()
        {
            var badgeGo = new GameObject("SteamInput.TitleBar.RightColumn.ActionGroup", typeof(RectTransform));
            
            // Sliced sprite: transparent fill with a green border
            var image = badgeGo.AddComponent<Image>();
            image.sprite = SpritesTitleBar.ActionGroupBorderSprite;
            image.type = Image.Type.Sliced;
            image.color = Color.white;
            image.raycastTarget = false;

            // Padding around the text; the HLG reports the label's preferred size as its own.
            var layout = badgeGo.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(
                SteamInputPalette.TitleBarActionGroupPaddingH,
                SteamInputPalette.TitleBarActionGroupPaddingH,
                SteamInputPalette.TitleBarActionGroupPaddingV,
                SteamInputPalette.TitleBarActionGroupPaddingV);
            layout.spacing = 0f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            // Hug the label width (same pattern as ButtonBuilder's auto-width mode): the fitter
            // runs after the parent layout and re-applies the preferred width, so the badge keeps
            // hugging its text even when the title bar row gets squeezed below its preferred size.
            var fitter = badgeGo.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

            // Green label
            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(badgeGo.transform, false);

            var label = labelGo.AddComponent<TextMeshProUGUI>();
            label.font = DefaultPalette.Font;
            label.fontSize = SteamInputPalette.TitleBarActionGroupFontSize;
            label.color = DefaultPalette.AccentColor;
            label.alignment = TextAlignmentOptions.Center;
            label.enableWordWrapping = false;
            label.overflowMode = TextOverflowModes.Overflow;
            label.raycastTarget = false;
            
            return badgeGo
                .AddComponent<ActionGroupLabelController>()
                .ViewModel(this._viewModel)
                .Label(label);
        }
    }
}