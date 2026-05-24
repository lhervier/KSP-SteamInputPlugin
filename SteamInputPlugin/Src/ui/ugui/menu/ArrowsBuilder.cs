using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using System;
using com.github.lhervier.ksp.ui.model;
using System.Security.Policy;

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

        public ArrowsController Create(UIConfigZone zone)
        {
            var go = new GameObject("Arrows", typeof(RectTransform));
            ArrowsController controller = go.AddComponent<ArrowsController>();
            controller.Initialize(_viewModel);
            controller.InitZone(zone);

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = SteamInputPalette.MenuArrowsSpacing;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            ButtonController upButtonController = this._buttonBuilder.Create(
                "Up", 
                "▲", 
                controller.MoveUp,
                !zone.First
            );
            upButtonController.transform.SetParent(go.transform, false);
            controller.InitUpButton(upButtonController);
            
            ButtonController downButtonController = this._buttonBuilder.Create(
                "Down", 
                "▼", 
                controller.MoveDown,
                !zone.Last
            );
            downButtonController.transform.SetParent(go.transform, false);
            controller.InitDownButton(downButtonController);

            return controller;
        }

        public class ArrowsController : BaseSteamInputController
        {
            private UIConfigZone _zone;
            private ButtonController _upButton;
            private ButtonController _downButton;

            public void InitZone(UIConfigZone zone)
            {
                _zone = zone;
            }

            public void InitUpButton(ButtonController upButton)
            {
                _upButton = upButton;
            }

            public void InitDownButton(ButtonController downButton)
            {
                _downButton = downButton;
            }

            public void MoveUp()
            {
                ViewModel?.MoveZoneUp(_zone);
            }

            public void MoveDown()
            {
                ViewModel?.MoveZoneDown(_zone);
            }

            public void UpdateZone(UIConfigZone zone)
            {
                _upButton?.SetInteractable(!zone.First);
                _downButton?.SetInteractable(!zone.Last);
            }
        }
    }
}
