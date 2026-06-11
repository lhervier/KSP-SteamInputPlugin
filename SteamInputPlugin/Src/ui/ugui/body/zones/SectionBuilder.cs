using UnityEngine;
using UnityEngine.UI;
using TMPro;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.model;
using com.github.lhervier.ksp.shared.ugui.styles;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.zones
{
    /// <summary>
    /// Displays one section of a zone body:
    ///   - A "NORMAL" / "↓ MODESHIFT" subheader (mockup .kstate)
    ///   - One activator row (mockup .krow) per binding of the section's group
    /// </summary>
    public class SectionBuilder : IUGUIBuilder<SectionController>
    {
        // =====================================
        // Build parameters
        // =====================================

        private CheatSheetViewModel _viewModel;
        public SectionBuilder ViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        private UISection _section;
        public SectionBuilder Section(UISection section)
        {
            this._section = section;
            return this;
        }

        // ==================================
        // Build
        // ==================================

        public SectionController Build()
        {
            var go = new GameObject("Mode", typeof(RectTransform));
            
            // Horizontal padding (Option A: padding on the container, not per-section)
            // Vertical padding-bottom matches the .kzone body breathing room.
            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(
                Mathf.RoundToInt(DefaultPalette.PaddingLeft),
                Mathf.RoundToInt(DefaultPalette.PaddingRight),
                0,
                (int)DefaultPalette.PaddingBottom
            );
            layout.spacing = SteamInputPalette.ModeSpacing;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            string label;
            Color textColor;
            if( _section.Modeshift )
            {
                label = "↓ " + ModLocalization.GetString("SteamInput_sectionModeshift").ToUpperInvariant();
                textColor = SteamInputPalette.ModeShiftColor;
            }
            else
            {
                label = ModLocalization.GetString("SteamInput_sectionNormal").ToUpperInvariant();
                textColor = SteamInputPalette.ModeNormalColor;
            }

            // A layer section keeps its state color but is tagged with the layer title,
            // e.g. "NORMAL (RIGHTCLICK)" / "↓ MODESHIFT (RIGHTCLICK)".
            if( !string.IsNullOrEmpty(_section.LayerTitle) )
            {
                label += " " + ModLocalization.GetString("SteamInput_sectionLayerSuffix", _section.LayerTitle).ToUpperInvariant();
            }

            // .kstate subheader as a child so activator rows can be stacked below it.
            var labelGo = new GameObject("SectionLabel", typeof(RectTransform));
            labelGo.transform.SetParent(go.transform, false);

            var sectionText = labelGo.AddComponent<TextMeshProUGUI>();
            sectionText.text = label;
            sectionText.font = DefaultPalette.Font;
            sectionText.fontSize = SteamInputPalette.ModeLabelFontSize;
            sectionText.fontStyle = FontStyles.Bold;
            sectionText.color = textColor;
            sectionText.alignment = TextAlignmentOptions.Left;
            sectionText.enableWordWrapping = false;
            sectionText.overflowMode = TextOverflowModes.Overflow;
            sectionText.raycastTarget = false;

            return go
                .AddComponent<SectionController>()
                .ViewModel(_viewModel)
                .HeaderLabel(labelGo)
                .GroupId(_section.GroupId);
        }
    }
}
