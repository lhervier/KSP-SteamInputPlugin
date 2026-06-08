using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.model;
using com.github.lhervier.ksp.uigui.shared.styles;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.zones
{
    /// <summary>
    /// Builds the body of one zone as an ordered list of sections: the normal section first,
    /// then one per modeshift group. Empty sections are skipped, so normal and modeshift are
    /// handled the same way (a section is empty when its group is not a mouse group and has no
    /// binding — see <see cref="CheatSheetViewModel.IsSectionEmpty"/>).
    /// </summary>
    public class ZoneBodyBuilder
    {
        private CheatSheetViewModel _viewModel;
        private SectionBuilder _modeBuilder;

        public ZoneBodyBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._modeBuilder = new SectionBuilder(viewModel);
        }

        public ZoneBodyController Create(UIPhysicalZone zone)
        {
            var go = new GameObject("Body", typeof(RectTransform));
            ZoneBodyController controller = go.AddComponent<ZoneBodyController>();
            controller.Initialize(_viewModel);
            controller.BindModeBuilder(_modeBuilder);

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

            controller.UpdateZone(zone);

            return controller;
        }

        public class ZoneBodyController : BaseSteamInputController
        {
            private SectionBuilder _sectionBuilder;
            private readonly Dictionary<string, SectionBuilder.SectionController> _sections
                = new Dictionary<string, SectionBuilder.SectionController>();

            public void BindModeBuilder(SectionBuilder builder)
            {
                this._sectionBuilder = builder;
            }

            public void UpdateZone(UIPhysicalZone zone)
            {
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
                        controller = _sectionBuilder.Create(section);
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
