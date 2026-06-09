using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.model;
using com.github.lhervier.ksp.shared.ugui.styles;
using com.github.lhervier.ksp.shared.ugui;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.zones
{
    /// <summary>
    /// Builds the body of one zone as an ordered list of sections: the normal section first,
    /// then one per modeshift group. Empty sections are skipped, so normal and modeshift are
    /// handled the same way (a section is empty when its group is not a mouse group and has no
    /// binding — see <see cref="CheatSheetViewModel.IsSectionEmpty"/>).
    /// </summary>
    public class ZoneBodyBuilder : IUGUIBuilder<ZoneBodyBuilder.ZoneBodyController>
    {
        // =======================================
        // Builder parameters
        // =======================================

        private CheatSheetViewModel _viewModel;
        public ZoneBodyBuilder ViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        private UIPhysicalZone _zone;
        public ZoneBodyBuilder Zone(UIPhysicalZone zone)
        {
            this._zone = zone;
            return this;
        }

        // ===================================
        // Build
        // ===================================

        public ZoneBodyController Build()
        {
            var go = new GameObject("Body", typeof(RectTransform));
            ZoneBodyController controller = go
                .AddComponent<ZoneBodyController>()
                .ViewModel(_viewModel);

            // Horizontal padding (Option A: padding on the container, not per-section)
            // Vertical padding-bottom matches the .kzone body breathing room.
            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(
                Mathf.RoundToInt(DefaultPalette.PaddingLeft),
                Mathf.RoundToInt(DefaultPalette.PaddingRight),
                0,
                Mathf.RoundToInt(DefaultPalette.PaddingBottom)
            );
            layout.spacing = SteamInputPalette.ZoneBodySpacing;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            return go
                .AddComponent<ZoneBodyController>()
                .ViewModel(_viewModel)
                .Zone(_zone);
        }

        public class ZoneBodyController : MonoBehaviour
        {
            private readonly Dictionary<string, SectionBuilder.SectionController> _sections
                = new Dictionary<string, SectionBuilder.SectionController>();

            // ==========================================
            // Life cycle
            // ==========================================
            
            private CheatSheetViewModel _viewModel;
            public ZoneBodyController ViewModel(CheatSheetViewModel viewModel)
            {
                this._viewModel = viewModel;
                return this;
            }

            private UIPhysicalZone _zone;
            public ZoneBodyController Zone(UIPhysicalZone zone)
            {
                _zone = zone;
                return this;
            }

            public void Start()
            {
                this.UpdateZone(_zone);
            }

            public void OnDestroy()
            {
            }

            // ========================================
            // Public API
            // ========================================

            public void UpdateZone(UIPhysicalZone zone)
            {
                _zone = zone;

                // Destroy sections that are no longer present.
                HashSet<string> desiredIds = new HashSet<string>();
                foreach( UISection section in zone.Sections )
                {
                    desiredIds.Add(section.GroupId);
                }
                List<string> toRemove = new List<string>();
                foreach( string groupId in _sections.Keys )
                {
                    if( !desiredIds.Contains(groupId) )
                    {
                        toRemove.Add(groupId);
                    }
                }
                foreach( string groupId in toRemove )
                {
                    Destroy(_sections[groupId].gameObject);
                    _sections.Remove(groupId);
                }

                // Create or update sections, then impose the visual order via SetSiblingIndex.
                for( int i = 0; i < zone.Sections.Count; i++ )
                {
                    UISection section = zone.Sections[i];
                    if( !_sections.TryGetValue(section.GroupId, out SectionBuilder.SectionController controller) )
                    {
                        controller = new SectionBuilder()
                            .ViewModel(_viewModel)
                            .Section(section)
                            .Build();
                        controller.transform.SetParent(gameObject.transform, false);
                        _sections[section.GroupId] = controller;
                    }
                    else
                    {
                        controller.UpdateGroupId(section.GroupId);
                    }
                    // The "NORMAL" subheader is hidden when the normal section stands alone; a
                    // modeshift subheader is always shown (even when it is the only section).
                    controller.SetHeaderVisible(section.Modeshift || zone.Sections.Count > 1);
                    controller.transform.SetSiblingIndex(i);
                }
            }
        }
    }
}
