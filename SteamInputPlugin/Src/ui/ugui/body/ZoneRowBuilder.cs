using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using com.github.lhervier.ksp.ui.model;
using System;

namespace com.github.lhervier.ksp.ui.ugui.body
{
    /// <summary>
    /// Displays one UIPhysicalZone:
    ///   - Header row with the zone label (e.g. "STICK GAUCHE")
    ///   - "NORMAL" section if the zone has a GroupId
    ///   - "↓ MODESHIFT" section if the zone has a ModeshiftGroupId
    /// Styled to match the mockup .kzone / .kzh / .kstate rules.
    /// </summary>
    public class ZoneRowBuilder
    {
        private CheatSheetViewModel _viewModel;
        private ZoneHeaderBuilder _zoneHeaderBuilder;
        private ZoneBodyBuilder _zoneBodyBuilder;

        public ZoneRowBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._zoneHeaderBuilder = new ZoneHeaderBuilder(viewModel);
            this._zoneBodyBuilder = new ZoneBodyBuilder(viewModel);
        }

        public ZoneRowController Create(UIPresetZone zone)
        {
            var zoneGo = new GameObject("PhysicalZone." + zone.Zone.Name, typeof(RectTransform));
            var controller = zoneGo.AddComponent<ZoneRowController>();
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

            _zoneHeaderBuilder.Create(zone).transform.SetParent(zoneGo.transform, false);
            _zoneBodyBuilder.Create(zone).transform.SetParent(zoneGo.transform, false);

            return controller;
        }

        public class ZoneRowController : BaseSteamInputController
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
