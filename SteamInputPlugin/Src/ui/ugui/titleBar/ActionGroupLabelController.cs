using UnityEngine;
using TMPro;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui.styles;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.titleBar
{
    /// <summary>
    /// Pushes the action group label from the ViewModel into a text component.
    /// Subscribes on Bind, unsubscribes on OnDestroy.
    /// </summary>
    public class ActionGroupLabelController : BaseSteamInputController
    {
        // ============================
        // Life cycle
        // ============================

        private TextMeshProUGUI _label;
        public ActionGroupLabelController WithLabelComponent(TextMeshProUGUI label)
        {
            this._label = label;
            return this;
        }

        public void Start()
        {
            this.ViewModel.OnActionGroupLabelChanged.Add(OnLabelChanged);
            OnLabelChanged(this.ViewModel.ActionGroupLabel);
        }

        public void OnDestroy()
        {
            this.ViewModel?.OnActionGroupLabelChanged.Remove(OnLabelChanged);
        }

        // ====================================
        // Methods bounds to events
        // ====================================

        private void OnLabelChanged(string value)
        {
            if (this._label != null)
            {
                this._label.text = value;
            }
        }
    }
}