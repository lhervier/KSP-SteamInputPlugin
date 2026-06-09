using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.model;
using System.Collections.Generic;
using com.github.lhervier.ksp.shared.ugui.styles;
using com.github.lhervier.ksp.shared.ugui;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.zones
{
    /// <summary>
    /// Displays one section of a zone body:
    ///   - A "NORMAL" / "↓ MODESHIFT" subheader (mockup .kstate)
    ///   - One activator row (mockup .krow) per binding of the section's group
    /// </summary>
    public class SectionBuilder : IUGUIBuilder<SectionBuilder.SectionController>
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

            var sectionText = labelGo.AddComponent<Text>();
            sectionText.text = label;
            sectionText.font = HighLogic.UISkin.font;
            sectionText.fontSize = SteamInputPalette.ModeLabelFontSize;
            sectionText.fontStyle = FontStyle.Bold;
            sectionText.color = textColor;
            sectionText.alignment = TextAnchor.MiddleLeft;
            sectionText.horizontalOverflow = HorizontalWrapMode.Overflow;
            sectionText.verticalOverflow = VerticalWrapMode.Overflow;
            sectionText.raycastTarget = false;

            return go
                .AddComponent<SectionController>()
                .ViewModel(_viewModel)
                .HeaderLabel(labelGo)
                .GroupId(_section.GroupId);
        }

        public class SectionController : MonoBehaviour
        {
            
            private MouseLineBuilder.MouseLineController _mouseLineController;
            private readonly List<ActivatorBuilder.ActivatorController> _rowControllers
                = new List<ActivatorBuilder.ActivatorController>();

            // =========================================
            // Life cycle
            // =========================================

            private CheatSheetViewModel _viewModel;
            public SectionController ViewModel(CheatSheetViewModel viewModel)
            {
                this._viewModel = viewModel;
                return this;
            }

            private GameObject _headerLabel;
            public SectionController HeaderLabel(GameObject headerLabel)
            {
                this._headerLabel = headerLabel;
                return this;
            }

            private string _groupId;
            public SectionController GroupId(string groupId)
            {
                this._groupId = groupId;
                return this;
            }

            public void Start()
            {
                this.UpdateGroupId(this._groupId);
            }

            // ==========================================
            // Public API
            // ==========================================

            /// <summary>Show/hide the "NORMAL" / "↓ MODESHIFT" subheader (kept at sibling index 0).</summary>
            public void SetHeaderVisible(bool visible)
            {
                if( _headerLabel != null )
                {
                    _headerLabel.SetActive(visible);
                }
            }

            public void UpdateGroupId(string groupId)
            {
                _groupId = groupId;
                
                // Rebuild the section content below the subheader (first child).
                foreach( ActivatorBuilder.ActivatorController row in _rowControllers )
                {
                    Destroy(row.gameObject);
                }
                _rowControllers.Clear();
                if( _mouseLineController != null )
                {
                    Destroy(_mouseLineController.gameObject);
                    _mouseLineController = null;
                }

                // Mouse-mode groups get a banner right after the subheader, above any rows.
                if( _viewModel.IsMouseGroup(groupId) )
                {
                    _mouseLineController = new MouseLineBuilder().Build();
                    _mouseLineController.transform.SetParent(gameObject.transform, false);
                    _mouseLineController.transform.SetSiblingIndex(1);
                }

                // Then one row per activator (e.g. a click on the joystick).
                foreach( UIActivator activator in _viewModel.GetActivators(groupId) )
                {
                    ActivatorBuilder.ActivatorController row = new ActivatorBuilder()
                        .ViewModel(_viewModel)
                        .Activator(activator)
                        .Build();
                    row.transform.SetParent(gameObject.transform, false);
                    row.transform.SetAsLastSibling();
                    _rowControllers.Add(row);
                }
            }
        }
    }
}
