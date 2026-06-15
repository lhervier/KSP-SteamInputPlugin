using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
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

        private TextMeshProUGUI _label;
        public GamepadLabelController WithLabel(TextMeshProUGUI label)
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