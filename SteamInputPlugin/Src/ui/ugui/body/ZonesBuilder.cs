using UnityEngine;
using UnityEngine.UI;
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
            controller.InitPhysicalZoneBuilder(_zoneRowBuilder);

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
            private Dictionary<GamepadZone, ZoneRowBuilder.ZoneRowController> _rows = new Dictionary<GamepadZone, ZoneRowBuilder.ZoneRowController>();

            public void InitPhysicalZoneBuilder(ZoneRowBuilder physicalZoneBuilder)
            {
                this._zoneRowBuilder = physicalZoneBuilder;
            }

            public void Start()
            {
                ViewModel?.OnPhysicalZonesChanged.Add(Sync);
                Sync(ViewModel?.PhysicalZones ?? null);
            }

            public void OnDestroy()
            {
                ViewModel?.OnPhysicalZonesChanged.Remove(Sync);
            }

            private void Sync(List<UIPhysicalZone> zones)
            {
                if( zones == null ) return;

                // 1. Set of keys present in the new list
                var newKeys = new HashSet<GamepadZone>();
                for (int i = 0; i < zones.Count; i++)
                {
                    if( zones[i].Visible )
                    {
                        newKeys.Add(zones[i].Zone);
                    }
                }

                // 2. Destroy rows whose zones are no longer present
                var toRemove = new List<GamepadZone>();
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

                // 3. Add or update rows for VISIBLE zones only, then apply the new order via SetSiblingIndex.
                int visibleIndex = 0;
                for (int i = 0; i < zones.Count; i++)
                {
                    var zone = zones[i];
                    if (!zone.Visible) continue;

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
        }
    }
}
