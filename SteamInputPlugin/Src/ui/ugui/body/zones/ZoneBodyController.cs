using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.model;
using com.github.lhervier.ksp.shared.ugui.styles;
using com.github.lhervier.ksp.shared.ugui;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.zones
{
    public class ZoneBodyController : MonoBehaviour
    {
        private readonly Dictionary<string, SectionController> _sections
            = new Dictionary<string, SectionController>();

        // ==========================================
        // Life cycle
        // ==========================================
        
        private CheatSheetViewModel _viewModel;
        public ZoneBodyController WithViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        private UIPhysicalZone _zone;
        public ZoneBodyController WithZone(UIPhysicalZone zone)
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
                if( !_sections.TryGetValue(section.GroupId, out SectionController controller) )
                {
                    controller = new SectionBuilder()
                        .WithViewModel(_viewModel)
                        .WithSection(section)
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
