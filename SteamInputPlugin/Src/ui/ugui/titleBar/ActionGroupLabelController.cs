using UnityEngine;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui.badge;
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

        private BadgeController _badge;
        public ActionGroupLabelController WithBadge(BadgeController badge)
        {
            this._badge = badge;
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
            if (this._badge != null)
            {
                this._badge.SetText(value);
            }
        }
    }
}