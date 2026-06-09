using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using UnityEngine.Events;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.titleBar
{
    public class GamepadLabelController : BaseSteamInputController
    {
        // =========================
        // Life cycle
        // =========================

        private Text _label;
        public GamepadLabelController Label(Text label)
        {
            this._label = label;
            return this;
        }

        public void Start()
        {
            this.ViewModel.OnGamepadLabelChanged.Add(OnLabelChanged);
            OnLabelChanged(this.ViewModel.GamepadLabel);
        }

        public void OnDestroy()
        {
            this.ViewModel?.OnGamepadLabelChanged.Remove(OnLabelChanged);
        }

        // =====================================
        // Methods bounds to events
        // =====================================

        private void OnLabelChanged(string value)
        {
            if (this._label != null)
            {
                this._label.text = value;
            }
        }
    }
}