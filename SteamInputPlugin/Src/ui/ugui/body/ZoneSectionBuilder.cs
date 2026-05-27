using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.model;
using System;
using System.Collections.Generic;

namespace com.github.lhervier.ksp.ui.ugui.body
{
    /// <summary>
    /// Displays one section of a zone body:
    ///   - A "NORMAL" / "↓ MODESHIFT" subheader (mockup .kstate)
    ///   - One activator row (mockup .krow) per binding of the section's group
    /// </summary>
    public class ZoneSectionBuilder
    {
        private const int SectionLabelFontSize = 10;

        private CheatSheetViewModel _viewModel;
        private ActivatorRowBuilder _activatorRowBuilder;

        public ZoneSectionBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._activatorRowBuilder = new ActivatorRowBuilder(viewModel);
        }

        public ZoneSectionController Create(String groupId, bool modeshift)
        {
            var go = new GameObject("ZoneSection", typeof(RectTransform));
            ZoneSectionController controller = go.AddComponent<ZoneSectionController>();
            controller.Initialize(_viewModel);
            controller.BindActivatorRowBuilder(_activatorRowBuilder);

            // Horizontal padding (Option A: padding on the container, not per-section)
            // Vertical padding-bottom matches the .kzone body breathing room.
            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingLeft),
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingRight),
                0,
                (int)SteamInputPalette.DefaultPaddingBottom
            );
            layout.spacing = SteamInputPalette.MainSectionSpacing;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            string label;
            Color textColor;
            if( modeshift )
            {
                label = "↓ " + ModLocalization.GetString("SteamInput_sectionModeshift").ToUpperInvariant();
                textColor = SteamInputPalette.SectionModeshift;
            }
            else
            {
                label = ModLocalization.GetString("SteamInput_sectionNormal").ToUpperInvariant();
                textColor = SteamInputPalette.SectionNormal;
            }

            // .kstate subheader as a child so activator rows can be stacked below it.
            var labelGo = new GameObject("SectionLabel", typeof(RectTransform));
            labelGo.transform.SetParent(go.transform, false);

            var sectionText = labelGo.AddComponent<Text>();
            sectionText.text = label;
            sectionText.font = HighLogic.UISkin.font;
            sectionText.fontSize = SectionLabelFontSize;
            sectionText.fontStyle = FontStyle.Bold;
            sectionText.color = textColor;
            sectionText.alignment = TextAnchor.MiddleLeft;
            sectionText.horizontalOverflow = HorizontalWrapMode.Overflow;
            sectionText.verticalOverflow = VerticalWrapMode.Overflow;
            sectionText.raycastTarget = false;

            controller.UpdateGroupId(groupId);

            return controller;
        }

        public class ZoneSectionController : BaseSteamInputController
        {
            private ActivatorRowBuilder _activatorRowBuilder;
            private readonly List<ActivatorRowBuilder.ActivatorRowController> _rowControllers
                = new List<ActivatorRowBuilder.ActivatorRowController>();

            public void BindActivatorRowBuilder(ActivatorRowBuilder builder)
            {
                this._activatorRowBuilder = builder;
            }

            public void UpdateGroupId(string groupId)
            {
                // Rebuild the rows: the section subheader (first child) is kept.
                foreach( ActivatorRowBuilder.ActivatorRowController row in _rowControllers )
                {
                    Destroy(row.gameObject);
                }
                _rowControllers.Clear();

                foreach( UIActivator activator in this.ViewModel.GetActivators(groupId) )
                {
                    ActivatorRowBuilder.ActivatorRowController row = _activatorRowBuilder.Create(activator);
                    row.transform.SetParent(gameObject.transform, false);
                    row.transform.SetAsLastSibling();
                    _rowControllers.Add(row);
                }
            }
        }
    }
}
