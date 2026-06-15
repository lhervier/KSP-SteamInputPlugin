using UnityEngine;
using TMPro;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using com.github.lhervier.ksp.steaminput.ui.model;
using com.github.lhervier.ksp.shared.ugui.styles;
using com.github.lhervier.ksp.shared.ugui;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.zones
{
    public class ZoneHeaderController : MonoBehaviour
    {
        // =====================================
        // Controller parameters
        // =====================================

        private TextMeshProUGUI _label;
        public ZoneHeaderController WithLabelComponent(TextMeshProUGUI label)
        {
            _label = label;
            return this;
        }

        // ====================================
        // Public API
        // ====================================

        public void UpdateZone(UIPhysicalZone zone)
        {
            _label.text = zone.Label;
        }
    }
}
