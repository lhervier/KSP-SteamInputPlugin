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
        private ModeBuilder _modeBuilder;

        public ZoneBodyBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._modeBuilder = new ModeBuilder(viewModel);
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
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingLeft),
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingRight),
                0,
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingBottom)
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
            private ModeBuilder _modeBuilder;
            private readonly Dictionary<string, ModeBuilder.ModeController> _modes
                = new Dictionary<string, ModeBuilder.ModeController>();

            public void BindModeBuilder(ModeBuilder builder)
            {
                this._modeBuilder = builder;
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
                foreach( string groupId in _modes.Keys )
                {
                    if( !desiredIds.Contains(groupId) )
                    {
                        toRemove.Add(groupId);
                    }
                }
                foreach( string groupId in toRemove )
                {
                    Destroy(_modes[groupId].gameObject);
                    _modes.Remove(groupId);
                }

                // Create or update sections, then impose the visual order via SetSiblingIndex.
                for( int i = 0; i < zone.Sections.Count; i++ )
                {
                    UISection section = zone.Sections[i];
                    if( !_modes.TryGetValue(section.GroupId, out ModeBuilder.ModeController controller) )
                    {
                        controller = _modeBuilder.Create(section.GroupId, section.Modeshift);
                        controller.transform.SetParent(gameObject.transform, false);
                        _modes[section.GroupId] = controller;
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
