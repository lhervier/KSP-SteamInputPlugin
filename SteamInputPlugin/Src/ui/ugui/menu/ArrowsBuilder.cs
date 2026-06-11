using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using System;
using com.github.lhervier.ksp.steaminput.ui.model;
using System.Security.Policy;
using com.github.lhervier.ksp.shared.ugui.button;
using com.github.lhervier.ksp.shared.ugui.styles;
using com.github.lhervier.ksp.shared.ugui;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.menu
{
    public class ArrowsBuilder : IUGUIBuilder<ArrowsController>
    {
        // ===============================================
        // Builder parameters
        // ===============================================

        private CheatSheetViewModel _viewModel;
        public ArrowsBuilder ViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        private UIConfigZone _zone;
        public ArrowsBuilder Zone(UIConfigZone zone)
        {
            this._zone = zone;
            return this;
        }

        
        // =========================================
        // Build
        // =========================================

        public ArrowsController Build()
        {
            var go = new GameObject("Arrows", typeof(RectTransform));
            
            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = SteamInputPalette.MenuArrowsSpacing;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            // Triangles when the font (or a fallback) provides them, plain arrows otherwise.
            ButtonController upButtonController = new ButtonBuilder()
                .ObjectName("Up")
                .Label(DefaultPalette.PickGlyph("▲", "↑"))
                .Interactable(!_zone.First)
                .Build();
            upButtonController.transform.SetParent(go.transform, false);

            ButtonController downButtonController = new ButtonBuilder()
                .ObjectName("Down")
                .Label(DefaultPalette.PickGlyph("▼", "↓"))
                .Interactable(!_zone.Last)
                .Build();
            downButtonController.transform.SetParent(go.transform, false);

            return go
                .AddComponent<ArrowsController>()
                .ViewModel(_viewModel)
                .Zone(_zone)
                .UpButton(upButtonController)
                .DownButton(downButtonController);
        }
    }
}
