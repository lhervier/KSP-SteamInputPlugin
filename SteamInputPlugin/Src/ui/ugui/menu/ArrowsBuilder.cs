using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using System;

namespace com.github.lhervier.ksp.ui.ugui.menu
{
    public class ArrowsBuilder
    {
        private CheatSheetViewModel _viewModel;
        private ButtonBuilder _buttonBuilder;

        public ArrowsBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._buttonBuilder = new ButtonBuilder(viewModel);
        }

        public ArrowsController Create(Action onUp, Action onDown, bool first, bool last)
        {
            var go = new GameObject("Arrows", typeof(RectTransform));
            ArrowsController controller = go.AddComponent<ArrowsController>();
            controller.Initialize(_viewModel);

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = SteamInputPalette.MenuArrowsSpacing;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            this._buttonBuilder.Create(
                "Up", 
                "▲", 
                onUp
            )
            .transform.SetParent(go.transform, false);
            this._buttonBuilder.Create(
                "Down", 
                "▼", 
                onDown
            )
            .transform.SetParent(go.transform, false);

            return controller;
        }

        public class ArrowsController : BaseSteamInputController
        {
        }
    }
}
