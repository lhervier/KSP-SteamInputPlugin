using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.sprites;
using com.github.lhervier.ksp.ui.model;
using System;

namespace com.github.lhervier.ksp.ui.ugui.body.zones
{
    /// <summary>
    /// Displays one UIPhysicalZone:
    ///   - Header row with the zone label (e.g. "STICK GAUCHE")
    ///   - "NORMAL" section if the zone has a GroupId
    ///   - "↓ MODESHIFT" section if the zone has a ModeshiftGroupId
    /// Styled to match the mockup .kzone / .kzh / .kstate rules.
    /// </summary>
    public class ZoneBuilder
    {
        private CheatSheetViewModel _viewModel;
        private ZoneHeaderBuilder _zoneHeaderBuilder;
        private ZoneBodyBuilder _zoneBodyBuilder;

        public ZoneBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._zoneHeaderBuilder = new ZoneHeaderBuilder(viewModel);
            this._zoneBodyBuilder = new ZoneBodyBuilder(viewModel);
        }

        public ZoneController Create(UIPresetZone zone)
        {
            var zoneGo = new GameObject("PhysicalZone." + zone.Zone.Name, typeof(RectTransform));
            var controller = zoneGo.AddComponent<ZoneController>();
            controller.Initialize(_viewModel);

            // Stack: header, body, bottom separator
            var layout = zoneGo.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = 0f;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var headerController = _zoneHeaderBuilder.Create(zone);
            headerController.transform.SetParent(zoneGo.transform, false);
            controller.BindZoneHeaderController(headerController);

            var bodyController = _zoneBodyBuilder.Create(zone);
            bodyController.transform.SetParent(zoneGo.transform, false);
            controller.BindZoneBodyController(bodyController);

            return controller;
        }

        public class ZoneController : BaseSteamInputController
        {
            private ZoneHeaderBuilder.ZoneHeaderController _zoneHeaderController;
            private ZoneBodyBuilder.ZoneBodyController _zoneBodyController;

            public void BindZoneHeaderController(ZoneHeaderBuilder.ZoneHeaderController zoneHeaderController)
            {
                this._zoneHeaderController = zoneHeaderController;
            }

            public void BindZoneBodyController(ZoneBodyBuilder.ZoneBodyController zoneBodyController)
            {
                _zoneBodyController = zoneBodyController;
            }

            public void UpdateZone(UIPresetZone zone)
            {
                _zoneHeaderController?.UpdateZone(zone);
                _zoneBodyController?.UpdateZone(zone);
            }
        }
    }
}
