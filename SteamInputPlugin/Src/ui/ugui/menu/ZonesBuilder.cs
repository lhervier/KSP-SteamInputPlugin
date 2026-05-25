using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.model;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using com.github.lhervier.ksp.ui.model;

namespace com.github.lhervier.ksp.ui.ugui.menu
{
    public class ZonesBuilder
    {
        private CheatSheetViewModel _viewModel;
        private ZoneRowBuilder _zoneRowBuilder;

        public ZonesBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._zoneRowBuilder = new ZoneRowBuilder(viewModel);
        }

        public ZonesController Create()
        {
            var go = new GameObject("Zones", typeof(RectTransform));
            ZonesController controller = go.AddComponent<ZonesController>();
            controller.Initialize(_viewModel);
            
            // VLG stacks the zone rows. spacing matches the outer menu so rows breathe like
            // title/separator above. The ZonesBinder calls SetSiblingIndex on rows to apply order.
            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = SteamInputPalette.MenuSpacing;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            controller.BindRowBuilder(_zoneRowBuilder);

            return controller;
        }

        /// <summary>
        /// Keeps a list of zone rows in sync with the ViewModel's PhysicalZones.
        /// Diff-and-patch: existing rows are reused (and updated via their controller), missing zones
        /// destroyed, new zones created. Reordering is applied via SetSiblingIndex.
        /// </summary>
        public class ZonesController : BaseSteamInputController
        {
            private ZoneRowBuilder _rowBuilder;
            private Dictionary<EGamepadZone, ZoneRowBuilder.ZoneRowController> _rows = new Dictionary<EGamepadZone, ZoneRowBuilder.ZoneRowController>();

            public void BindRowBuilder(ZoneRowBuilder rowBuilder)
            {
                this._rowBuilder = rowBuilder;
            }

            public void Start()
            {
                this.ViewModel.OnConfigZonesChanged.Add(Sync);
                Sync(this.ViewModel.ConfigZones);
            }

            public void OnDestroy()
            {
                this.ViewModel?.OnConfigZonesChanged.Remove(Sync);
            }

            private void Sync(List<UIConfigZone> zones)
            {
                // 1. Set of keys present in the new list
                var newKeys = new HashSet<EGamepadZone>();
                for (int i = 0; i < zones.Count; i++)
                {
                    newKeys.Add(zones[i].Zone);
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

                // 3. Add or update rows, then apply the new order via SetSiblingIndex
                for (int i = 0; i < zones.Count; i++)
                {
                    var zone = zones[i];
                    if (!this._rows.TryGetValue(zone.Zone, out ZoneRowBuilder.ZoneRowController row))
                    {
                        row = this._rowBuilder.Create(zone);
                        row.transform.SetParent(this.transform, false);
                        this._rows[zone.Zone] = row;
                    }
                    else
                    {
                        row.UpdateZone(zone);
                    }
                    row.transform.SetSiblingIndex(i);
                }
            }
        }
    }
}