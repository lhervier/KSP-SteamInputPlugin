using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.steaminput;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using com.github.lhervier.ksp.steaminput.ui.model;
using System;
using com.github.lhervier.ksp.shared.ugui;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.zones
{
    /// <summary>
    /// Displays one UIPhysicalZone:
    ///   - Header row with the zone label (e.g. "STICK GAUCHE")
    ///   - "NORMAL" section if the zone has a GroupId
    ///   - "↓ MODESHIFT" section if the zone has a ModeshiftGroupId
    /// Styled to match the mockup .kzone / .kzh / .kstate rules.
    /// </summary>
    public class ZoneBuilder : IUGUIBuilder<ZoneController>
    {
        // =====================================
        // Builder parameters
        // =====================================

        private CheatSheetViewModel _viewModel;
        public ZoneBuilder ViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        private UIPhysicalZone _zone;
        public ZoneBuilder Zone(UIPhysicalZone zone)
        {
            this._zone = zone;
            return this;
        }

        // ====================================
        // Build
        // ====================================

        public ZoneController Build()
        {
            var zoneGo = new GameObject("PhysicalZone." + _zone.Zone.Name, typeof(RectTransform));
            var controller = zoneGo.AddComponent<ZoneController>();
            controller.ViewModel(_viewModel);

            // Stack: header, body, bottom separator
            var layout = zoneGo.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = 0f;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var headerController = new ZoneHeaderBuilder()
                .ViewModel(_viewModel)
                .Zone(_zone)
                .Build();
            headerController.transform.SetParent(zoneGo.transform, false);
            controller.BindZoneHeaderController(headerController);

            var bodyController = new ZoneBodyBuilder()
                .ViewModel(_viewModel)
                .Zone(_zone)
                .Build();
            bodyController.transform.SetParent(zoneGo.transform, false);
            controller.BindZoneBodyController(bodyController);

            return controller;
        }
    }
}
