using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.model;
using System.Collections.Generic;

namespace com.github.lhervier.ksp.ui.ugui.body.zones
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
        private ZoneSectionBuilder _zoneSectionBuilder;

        public ZoneBodyBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._zoneSectionBuilder = new ZoneSectionBuilder(viewModel);
        }

        public ZoneBodyController Create(UIPresetZone zone)
        {
            var go = new GameObject("Body", typeof(RectTransform));
            ZoneBodyController controller = go.AddComponent<ZoneBodyController>();
            controller.Initialize(_viewModel);
            controller.BindZoneSectionBuilder(_zoneSectionBuilder);

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

            controller.UpdateZone(zone);

            return controller;
        }

        public class ZoneBodyController : BaseSteamInputController
        {
            private ZoneSectionBuilder _zoneSectionBuilder;
            private readonly Dictionary<string, ZoneSectionBuilder.ZoneSectionController> _sections
                = new Dictionary<string, ZoneSectionBuilder.ZoneSectionController>();

            public void BindZoneSectionBuilder(ZoneSectionBuilder builder)
            {
                this._zoneSectionBuilder = builder;
            }

            public void UpdateZone(UIPresetZone zone)
            {
                // The desired sections in display order: normal group first, then each modeshift
                // group, keeping only the non-empty ones.
                List<SectionKey> sections = new List<SectionKey>();
                if( !ViewModel.IsSectionEmpty(zone.GroupId) )
                {
                    sections.Add(new SectionKey(zone.GroupId, false));
                }
                foreach( string modeshiftGroupId in zone.ModeshiftGroupIds )
                {
                    if( !ViewModel.IsSectionEmpty(modeshiftGroupId) )
                    {
                        sections.Add(new SectionKey(modeshiftGroupId, true));
                    }
                }

                // Destroy sections that are no longer present.
                HashSet<string> desiredIds = new HashSet<string>();
                foreach( SectionKey section in sections )
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
                for( int i = 0; i < sections.Count; i++ )
                {
                    SectionKey section = sections[i];
                    if( !_sections.TryGetValue(section.GroupId, out ZoneSectionBuilder.ZoneSectionController controller) )
                    {
                        controller = _zoneSectionBuilder.Create(section.GroupId, section.Modeshift);
                        controller.transform.SetParent(gameObject.transform, false);
                        _sections[section.GroupId] = controller;
                    }
                    else
                    {
                        controller.UpdateGroupId(section.GroupId);
                    }
                    // The "NORMAL" subheader is hidden when the normal section stands alone; a
                    // modeshift subheader is always shown (even when it is the only section).
                    controller.SetHeaderVisible(section.Modeshift || sections.Count > 1);
                    controller.transform.SetSiblingIndex(i);
                }
            }

            private struct SectionKey
            {
                public readonly string GroupId;
                public readonly bool Modeshift;

                public SectionKey(string groupId, bool modeshift)
                {
                    GroupId = groupId;
                    Modeshift = modeshift;
                }
            }
        }
    }
}
