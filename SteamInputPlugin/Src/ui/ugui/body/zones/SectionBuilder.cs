using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.model;
using System.Collections.Generic;
using com.github.lhervier.ksp.shared.ugui.styles;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.zones
{
    /// <summary>
    /// Displays one section of a zone body:
    ///   - A "NORMAL" / "↓ MODESHIFT" subheader (mockup .kstate)
    ///   - One activator row (mockup .krow) per binding of the section's group
    /// </summary>
    public class SectionBuilder
    {
        private CheatSheetViewModel _viewModel;
        private ActivatorBuilder _activatorRowBuilder;
        private MouseLineBuilder _mouseLineBuilder;

        public SectionBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._activatorRowBuilder = new ActivatorBuilder(viewModel);
            this._mouseLineBuilder = new MouseLineBuilder(viewModel);
        }

        public SectionController Create(UISection section)
        {
            var go = new GameObject("Mode", typeof(RectTransform));
            SectionController controller = go.AddComponent<SectionController>();
            controller.BindViewModel(_viewModel);
            controller.BindActivatorRowBuilder(_activatorRowBuilder);
            controller.BindMouseLineBuilder(_mouseLineBuilder);

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
            if( section.Modeshift )
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
            if( !string.IsNullOrEmpty(section.LayerTitle) )
            {
                label += " " + ModLocalization.GetString("SteamInput_sectionLayerSuffix", section.LayerTitle).ToUpperInvariant();
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

            controller.BindHeaderLabel(labelGo);
            controller.UpdateGroupId(section.GroupId);

            return controller;
        }

        public class SectionController : BaseSteamInputController
        {
            private ActivatorBuilder _activatorRowBuilder;
            private MouseLineBuilder _mouseLineBuilder;
            private GameObject _headerLabel;
            
            private MouseLineBuilder.MouseLineController _mouseLineController;
            private readonly List<ActivatorBuilder.ActivatorController> _rowControllers
                = new List<ActivatorBuilder.ActivatorController>();

            public void BindActivatorRowBuilder(ActivatorBuilder builder)
            {
                this._activatorRowBuilder = builder;
            }

            public void BindMouseLineBuilder(MouseLineBuilder builder)
            {
                this._mouseLineBuilder = builder;
            }

            public void BindHeaderLabel(GameObject headerLabel)
            {
                this._headerLabel = headerLabel;
            }

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
                if( this.ViewModel.IsMouseGroup(groupId) )
                {
                    _mouseLineController = _mouseLineBuilder.Create();
                    _mouseLineController.transform.SetParent(gameObject.transform, false);
                    _mouseLineController.transform.SetSiblingIndex(1);
                }

                // Then one row per activator (e.g. a click on the joystick).
                foreach( UIActivator activator in this.ViewModel.GetActivators(groupId) )
                {
                    ActivatorBuilder.ActivatorController row = _activatorRowBuilder.Create(activator);
                    row.transform.SetParent(gameObject.transform, false);
                    row.transform.SetAsLastSibling();
                    _rowControllers.Add(row);
                }
            }
        }
    }
}
