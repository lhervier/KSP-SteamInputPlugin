using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using System;
using com.github.lhervier.ksp.steaminput.ui.model;
using System.Security.Policy;
using com.github.lhervier.ksp.shared.ugui.button;
using com.github.lhervier.ksp.shared.ugui;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.menu
{
    public class ArrowsController : BaseSteamInputController
    {
        // ==========================================
        // Life cycle
        // ==========================================

        private UIConfigZone _zone;
        public ArrowsController Zone(UIConfigZone zone)
        {
            _zone = zone;
            return this;
        }

        private ButtonController _upButton;
        public ArrowsController UpButton(ButtonController upButton)
        {
            _upButton = upButton;
            return this;
        }

        private ButtonController _downButton;
        public ArrowsController DownButton(ButtonController downButton)
        {
            _downButton = downButton;
            return this;
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

        // ==========================================
        // Methods bound to events
        // ==========================================

        private void MoveUp()
        {
            ViewModel?.MoveZoneUp(_zone);
        }

        private void MoveDown()
        {
            ViewModel?.MoveZoneDown(_zone);
        }

        // ==========================================
        // Public API
        // ==========================================

        public void UpdateZone(UIConfigZone zone)
        {
            _upButton?.SetInteractable(!zone.First);
            _downButton?.SetInteractable(!zone.Last);
        }
    }
}
