using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using com.github.lhervier.ksp.ui.model;
using System;

namespace com.github.lhervier.ksp.ui.ugui.menu
{
    public class ZoneRowBuilder
    {

        private CheatSheetViewModel _viewModel;
        private CheckboxBuilder _checkBoxBuilder;
        private ZoneLabelBuilder _zoneLabelBuilder;
        private ArrowsBuilder _arrowsBuilder;

        public ZoneRowBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._checkBoxBuilder = new CheckboxBuilder(viewModel);
            this._zoneLabelBuilder = new ZoneLabelBuilder(viewModel);
            this._arrowsBuilder = new ArrowsBuilder(viewModel);
        }

        public ZoneRowController Create(UIPhysicalZone zone, bool first, bool last)
        {
            var rowGo = new GameObject("Zone." + zone.Zone.Name, typeof(RectTransform));
            var controller = rowGo.AddComponent<ZoneRowController>();
            controller.Initialize(_viewModel);
            
            // Horizontal: checkbox + label (greedy) + arrows
            var layout = rowGo.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = SteamInputPalette.DefaultSpacing;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            // Controller used to propagate changes from the viewModel into the GameObjects
            
            var checkboxGo = this._checkBoxBuilder.Create(
                zone.Visible,
                _ => this._viewModel.ToggleZoneVisibility(zone),
                controller
            );
            checkboxGo.transform.SetParent(rowGo.transform, false);

            var labelGo = this._zoneLabelBuilder.Create(
                zone.Label,
                controller
            );
            labelGo.transform.SetParent(rowGo.transform, false);

            this._arrowsBuilder.Create(
                () => this._viewModel.MoveZoneUp(zone),
                () => this._viewModel.MoveZoneDown(zone),
                first,
                last
            )
            .transform.SetParent(rowGo.transform, false);

            return controller;
        }

        public class ZoneRowController : BaseSteamInputController
        {
            private GameObject _checkmark;
            private Text _label;

            public void InitCheckmark(GameObject checkmark)
            {
                this._checkmark = checkmark;
            }

            public void InitLabel(Text label)
            {
                this._label = label;
            }

            public void UpdateZone(UIPhysicalZone zone)
            {
                if (this._checkmark != null)
                {
                    this._checkmark.SetActive(zone.Visible);
                }
                if (this._label != null)
                {
                    this._label.text = zone.Label;
                }
            }
        }
    }
}
