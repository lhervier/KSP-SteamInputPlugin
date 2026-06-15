using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.steaminput;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.model;
using System.Collections.Generic;
using com.github.lhervier.ksp.shared.ugui;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.zones
{
    /// <summary>
    /// Container that stacks one PhysicalZone per UIPhysicalZone from the ViewModel.
    /// Only visible zones are rendered (Visible flag from the toggle menu).
    /// </summary>
    public class ZoneListBuilder : IUGUIBuilder<ZoneListController>
    {
        // ============================================
        // Builder parameters
        // ============================================

        private CheatSheetViewModel _viewModel;
        public ZoneListBuilder WithViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        // =======================================
        // Build
        // =======================================

        public ZoneListController Build()
        {
            var go = new GameObject("PhysicalZones", typeof(RectTransform));
            
            // VLG stacks the zones back-to-back. No spacing — each zone has its own bottom separator.
            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = 0f;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            return go
                .AddComponent<ZoneListController>()
                .WithViewModel(_viewModel);;
        }
    }
}
