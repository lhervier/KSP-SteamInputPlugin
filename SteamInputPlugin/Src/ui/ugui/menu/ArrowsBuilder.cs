using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using System;
using com.github.lhervier.ksp.steaminput.ui.model;
using System.Security.Policy;
using com.github.lhervier.ksp.shared.ugui.button;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.menu
{
    public class ArrowsBuilder
    {
        private CheatSheetViewModel _viewModel;
        private ButtonBuilder _buttonBuilder;
        
        public ArrowsBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._buttonBuilder = new ButtonBuilder();
        }

        public ArrowsController Create(UIConfigZone zone)
        {
            var go = new GameObject("Arrows", typeof(RectTransform));
            ArrowsController controller = go.AddComponent<ArrowsController>();
            controller.BindViewModel(_viewModel);
            controller.BindZone(zone);

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = SteamInputPalette.MenuArrowsSpacing;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            ButtonController upButtonController = _buttonBuilder
                .ObjectName("Up")
                .Label("▲")
                .Interactable(!zone.First)
                .Build();
            upButtonController.transform.SetParent(go.transform, false);
            controller.BindUpButton(upButtonController);
            
            ButtonController downButtonController = _buttonBuilder
                .ObjectName("Down")
                .Label("▼")
                .Interactable(!zone.Last)
                .Build();
            downButtonController.transform.SetParent(go.transform, false);
            controller.BindDownButton(downButtonController);

            return controller;
        }

        public class ArrowsController : BaseSteamInputController
        {
            private UIConfigZone _zone;
            private ButtonController _upButton;
            private ButtonController _downButton;

            public void BindZone(UIConfigZone zone)
            {
                _zone = zone;
            }

            public void BindUpButton(ButtonController upButton)
            {
                _upButton = upButton;
            }

            public void BindDownButton(ButtonController downButton)
            {
                _downButton = downButton;
            }

            public void Start()
            {
                if( _upButton != null )
                {
                    _upButton.OnClick.Add(MoveUp);
                }
                if( _downButton != null )
                {
                    _downButton.OnClick.Add(MoveDown);
                }
            }

            public void OnDestroy()
            {
                if( _upButton != null )
                {
                    _upButton.OnClick.Remove(MoveUp);
                }
                if( _downButton != null )
                {
                    _downButton.OnClick.Remove(MoveDown);
                }
            }

            private void MoveUp()
            {
                ViewModel?.MoveZoneUp(_zone);
            }

            private void MoveDown()
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
