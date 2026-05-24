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
    public class ZoneBodyBuilder
    {
        private CheatSheetViewModel _viewModel;
        private ZoneSectionBuilder _zoneSectionBuilder;

        public ZoneBodyBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._zoneSectionBuilder = new ZoneSectionBuilder(viewModel);
        }

        public ZoneBodyController Create(UIActionGroupZone zone)
        {
            var go = new GameObject("Body", typeof(RectTransform));
            ZoneBodyController controller = go.AddComponent<ZoneBodyController>();
            controller.Initialize(_viewModel);

            // Horizontal padding (Option A: padding on the container, not per-section)
            // Vertical padding-bottom matches the .kzone body breathing room.
            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingLeft),
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingRight),
                0,
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingBottom)
            );
            layout.spacing = SteamInputPalette.ZoneSectionSpacing;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            if (!string.IsNullOrEmpty(zone.GroupId))
            {
                ZoneSectionBuilder.ZoneSectionController normalController =_zoneSectionBuilder.Create(zone, false);
                normalController.transform.SetParent(go.transform);
                controller.InitNormalSectionController(normalController);
            }

            if (!string.IsNullOrEmpty(zone.ModeshiftGroupId))
            {
                ZoneSectionBuilder.ZoneSectionController modeshiftController =_zoneSectionBuilder.Create(zone, true);
                modeshiftController.transform.SetParent(go.transform);
                controller.InitModeshiftSectionController(modeshiftController);
            }

            return controller;
        }

        public class ZoneBodyController : BaseSteamInputController
        {
            private ZoneSectionBuilder _zoneSectionBuilder;
            private ZoneSectionBuilder.ZoneSectionController _normalSectionController;
            private ZoneSectionBuilder.ZoneSectionController _modeshiftSectionController;

            public void InitZoneSectionBuilder(ZoneSectionBuilder builder)
            {
                this._zoneSectionBuilder = builder;
            }

            public void InitNormalSectionController(ZoneSectionBuilder.ZoneSectionController normalSectionController)
            {
                _normalSectionController = normalSectionController;
            }

            public void InitModeshiftSectionController(ZoneSectionBuilder.ZoneSectionController modeshiftSectionController)
            {
                _modeshiftSectionController = modeshiftSectionController;
            }

            public void UpdateZone(UIActionGroupZone zone)
            {
                if( zone.GroupId == null )
                {
                    Destroy(_normalSectionController?.gameObject);
                }
                else
                {
                    if( _normalSectionController == null )
                    {
                        _normalSectionController = _zoneSectionBuilder.Create(zone, false);
                    }
                    else
                    {
                        _normalSectionController.UpdateZone(zone);
                    }
                }

                if( zone.ModeshiftGroupId == null )
                {
                    Destroy(_modeshiftSectionController?.gameObject);
                }
                else
                {
                    if( _modeshiftSectionController == null )
                    {
                        _modeshiftSectionController = _zoneSectionBuilder.Create(zone, true);
                    }
                    else
                    {
                        _modeshiftSectionController.UpdateZone(zone);
                    }
                }
            }
        }
    }
}
