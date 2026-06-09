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
    public class ZoneBodyBuilder : IUGUIBuilder<ZoneBodyController>
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
    }
}
