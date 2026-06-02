using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.model;
using System.Collections.Generic;

namespace com.github.lhervier.ksp.ui.ugui.body.zones
{
    /// <summary>
    /// Container that stacks one PhysicalZone per UIPhysicalZone from the ViewModel.
    /// Only visible zones are rendered (Visible flag from the toggle menu).
    /// </summary>
    public class ZoneListBuilder
    {
        private CheatSheetViewModel _viewModel;
        private ZoneBuilder _zoneBuilder;
        private EmptyConfigBuilder _emptyConfigBuilder;

        public ZoneListBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._zoneBuilder = new ZoneBuilder(viewModel);
            this._emptyConfigBuilder = new EmptyConfigBuilder(viewModel);
        }

        public ZoneListController Create()
        {
            var go = new GameObject("PhysicalZones", typeof(RectTransform));
            var controller = go.AddComponent<ZoneListController>();
            controller.Initialize(_viewModel);
            controller.BindZoneBuilder(_zoneBuilder);
            controller.BindEmptyConfigBuilder(_emptyConfigBuilder);

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

        public class ZoneListController : BaseSteamInputController
        {
            private ZoneBuilder _zoneBuilder;
            private EmptyConfigBuilder _emptyConfigBuilder;

            private Dictionary<EGamepadZone, ZoneBuilder.ZoneController> _zoneControllers = new Dictionary<EGamepadZone, ZoneBuilder.ZoneController>();
            private EmptyConfigBuilder.EmptyConfigController _emptyConfigController;

            public void BindZoneBuilder(ZoneBuilder zoneRowBuilder)
            {
                this._zoneBuilder = zoneRowBuilder;
            }

            public void BindEmptyConfigBuilder(EmptyConfigBuilder builder)
            {
                this._emptyConfigBuilder = builder;
            }

            public void Start()
            {
                ViewModel?.OnGamepadConfigNameChanged.Add(OnGamepadConfigNameChanged);
                OnGamepadConfigNameChanged(ViewModel?.GamepadConfigName ?? string.Empty);
                
                ViewModel?.OnPhysicalZonesChanged.Add(OnPresetZonesChanged);
                OnPresetZonesChanged(ViewModel?.PhysicalZones ?? null);
            }

            public void OnDestroy()
            {
                ViewModel?.OnPhysicalZonesChanged.Remove(OnPresetZonesChanged);
                ViewModel?.OnGamepadConfigNameChanged.Remove(OnGamepadConfigNameChanged);
            }

            private void OnGamepadConfigNameChanged(string config)
            {
                if( _emptyConfigController == null )
                {
                    _emptyConfigController = _emptyConfigBuilder.Create();
                    _emptyConfigController.transform.SetParent(gameObject.transform);
                }

                bool showEmpty = string.IsNullOrEmpty(config);
                _emptyConfigController.gameObject.SetActive(showEmpty);
                if( _zoneControllers != null )
                {
                    foreach(ZoneBuilder.ZoneController zoneController in _zoneControllers.Values )
                    {
                        zoneController.gameObject.SetActive(!showEmpty);
                    }
                }
            }

            private void OnPresetZonesChanged(List<UIPhysicalZone> zones)
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
                foreach (var pair in this._zoneControllers)
                {
                    if (!newKeys.Contains(pair.Key))
                    {
                        toRemove.Add(pair.Key);
                    }
                }
                foreach (var key in toRemove)
                {
                    Destroy(this._zoneControllers[key].gameObject);
                    this._zoneControllers.Remove(key);
                }

                // 3. Add or update rows for renderable zones, then apply the new order via SetSiblingIndex.
                int visibleIndex = 0;
                for (int i = 0; i < zones.Count; i++)
                {
                    var zone = zones[i];
                    if (!ShouldRender(zone)) continue;

                    if (!this._zoneControllers.TryGetValue(zone.Zone, out ZoneBuilder.ZoneController zoneController))
                    {
                        zoneController = this._zoneBuilder.Create(zone);
                        zoneController.transform.SetParent(this.transform, false);
                        zoneController.gameObject.SetActive(!string.IsNullOrEmpty(SteamInputGlobalSettings.GetControllerConfigName()));
                        this._zoneControllers[zone.Zone] = zoneController;
                    }
                    else
                    {
                        zoneController.UpdateZone(zone);
                        zoneController.gameObject.SetActive(!string.IsNullOrEmpty(SteamInputGlobalSettings.GetControllerConfigName()));
                    }
                    zoneController.transform.SetSiblingIndex(visibleIndex);
                    visibleIndex++;
                }
            }

            private bool ShouldRender(UIPhysicalZone zone)
            {
                // Skip zones whose sections are all empty (incl. zones with no group at all).
                return ViewModel.HasNonEmptySection(zone);
            }
        }
    }
}
