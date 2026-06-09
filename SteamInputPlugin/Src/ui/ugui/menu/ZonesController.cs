using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.steaminput.model;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using com.github.lhervier.ksp.steaminput.ui.model;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.menu
{
    /// <summary>
    /// Keeps a list of zone rows in sync with the ViewModel's PhysicalZones.
    /// Diff-and-patch: existing rows are reused (and updated via their controller), missing zones
    /// destroyed, new zones created. Reordering is applied via SetSiblingIndex.
    /// </summary>
    public class ZonesController : BaseSteamInputController
    {
        private Dictionary<EGamepadZone, ZoneRowController> _rows = new Dictionary<EGamepadZone, ZoneRowController>();

        // =================================
        // Life cycle
        // =================================
        
        public void Start()
        {
            this.ViewModel.OnConfigZonesChanged.Add(Sync);
            Sync(this.ViewModel.ConfigZones);
        }

        public void OnDestroy()
        {
            this.ViewModel?.OnConfigZonesChanged.Remove(Sync);
        }

        // ==========================================
        // Methods bound to events
        // ==========================================

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
                if (!this._rows.TryGetValue(zone.Zone, out ZoneRowController row))
                {
                    row = new ZoneRowBuilder()
                        .ViewModel(ViewModel)
                        .Zone(zone)
                        .Build();
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