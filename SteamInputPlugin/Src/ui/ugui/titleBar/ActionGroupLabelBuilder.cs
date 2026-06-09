using UnityEngine;
using UnityEngine.UI;
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

            // Padding around the text; badge size driven by content + padding via the HLG's reported preferredSize
            var layout = badgeGo.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(5, 5, 2, 2);
            layout.spacing = 0f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            // Green label
            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(badgeGo.transform, false);

            var label = labelGo.AddComponent<Text>();
            label.font = HighLogic.UISkin.font;
            label.fontSize = 10;
            label.color = DefaultPalette.AccentColor;
            label.alignment = TextAnchor.MiddleCenter;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;
            
            return badgeGo
                .AddComponent<ActionGroupLabelController>()
                .ViewModel(this._viewModel)
                .Label(label);
        }
    }
}