using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.model;
using System.Collections.Generic;

namespace com.github.lhervier.ksp.ui.ugui.body
{
    /// <summary>
    /// Container that stacks one PhysicalZone per UIPhysicalZone from the ViewModel.
    /// Only visible zones are rendered (Visible flag from the toggle menu).
    /// </summary>
    public class ZonesBuilder
    {
        private CheatSheetViewModel _viewModel;
        private ZoneRowBuilder _zoneRowBuilder;

        public ZonesBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._zoneRowBuilder = new ZoneRowBuilder(viewModel);
        }

        public PhysicalZonesController Create()
        {
            var go = new GameObject("PhysicalZones", typeof(RectTransform));
            var controller = go.AddComponent<PhysicalZonesController>();
            controller.Initialize(_viewModel);
            controller.BindPhysicalZoneBuilder(_zoneRowBuilder);

            // VLG stacks the zones back-to-back. No spacing — each zone has its own bottom separator.
            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = 0f;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            return controller;
        }

        public class PhysicalZonesController : BaseSteamInputController
        {
            private ZoneRowBuilder _zoneRowBuilder;
            private Dictionary<EGamepadZone, ZoneRowBuilder.ZoneRowController> _rows = new Dictionary<EGamepadZone, ZoneRowBuilder.ZoneRowController>();

            public void BindPhysicalZoneBuilder(ZoneRowBuilder physicalZoneBuilder)
            {
                this._zoneRowBuilder = physicalZoneBuilder;
            }

            public void Start()
            {
                ViewModel?.OnPresetZonesChanged.Add(Sync);
                Sync(ViewModel?.PresetZones ?? null);
            }

            public void OnDestroy()
            {
                ViewModel?.OnPresetZonesChanged.Remove(Sync);
            }

            private void Sync(List<UIPresetZone> zones)
            {
                if( zones == null ) return;

                // A zone is rendered in the body iff the user has set it visible AND it has at least
                // one non-empty section in the current action group. The viewModel includes ALL
                // zones in _physicalZones (so the menu can configure them globally), so we filter here.

                // 1. Set of keys to render
                var newKeys = new HashSet<EGamepadZone>();
                for (int i = 0; i < zones.Count; i++)
                {
                    if (ShouldRender(zones[i]))
                    {
                        newKeys.Add(zones[i].Zone);
                    }
                }

                // 2. Destroy rows whose zones are no longer present
                var toRemove = new List<EGamepadZone>();
                foreach (var pair in this._rows)
                {
                    if (!newKeys.Contains(pair.Key))
                    {
                        toRemove.Add(pair.Key);
                    }
                }
                foreach (var key in toRemove)
                {
                    Destroy(this._rows[key].gameObject);
                    this._rows.Remove(key);
                }

                // 3. Add or update rows for renderable zones, then apply the new order via SetSiblingIndex.
                int visibleIndex = 0;
                for (int i = 0; i < zones.Count; i++)
                {
                    var zone = zones[i];
                    if (!ShouldRender(zone)) continue;

                    if (!this._rows.TryGetValue(zone.Zone, out ZoneRowBuilder.ZoneRowController row))
                    {
                        row = this._zoneRowBuilder.Create(zone);
                        row.transform.SetParent(this.transform, false);
                        this._rows[zone.Zone] = row;
                    }
                    else
                    {
                        row.UpdateZone(zone);
                    }
                    row.transform.SetSiblingIndex(visibleIndex);
                    visibleIndex++;
                }
            }

            private bool ShouldRender(UIPresetZone zone)
            {
                // Skip zones whose sections are all empty (incl. zones with no group at all).
                return ViewModel.HasNonEmptySection(zone);
            }
        }
    }
}
