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
using com.github.lhervier.ksp.shared.ugui;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.menu
{
    public class ZonesBuilder : IUGUIBuilder<ZonesController>
    {
        private CheatSheetViewModel _viewModel;
        public ZonesBuilder ViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        public ZonesController Build()
        {
            var go = new GameObject("Zones", typeof(RectTransform));
            
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

            return go
                .AddComponent<ZonesController>()
                .ViewModel(_viewModel);
        }
    }
}