using UnityEngine;
using UnityEngine.UI;
using TMPro;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.styles;
using com.github.lhervier.ksp.shared;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.zones
{
    /// <summary>
    /// Builds the "Mouse — Free movement" banner (mockup .kmouse-line), shown at the start
    /// of a section whose group uses a mouse mode (joystick_mouse / absolute_mouse).
    /// </summary>
    public class MouseLineBuilder : IUGUIBuilder<MouseLineBuilder.MouseLineController>
    {
        // ===================================
        // Build
        // ===================================

        public MouseLineController Build()
        {
            var go = new GameObject("MouseLine", typeof(RectTransform));

            // Horizontal inset is provided by the section container; only the vertical
            // breathing room (.kmouse-line padding-y) is carried here.
            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(
                0, 0,
                Mathf.RoundToInt(SteamInputPalette.MouseLinePaddingV),
                Mathf.RoundToInt(SteamInputPalette.MouseLinePaddingV));
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            var labelGo = new GameObject("Text", typeof(RectTransform));
            labelGo.transform.SetParent(go.transform, false);

            var label = UGUILabels.AddLabel(labelGo);
            label.text = ModLocalization.GetString("mouseMovement");
            label.fontSize = SteamInputPalette.MouseLineFontSize;
            label.fontStyle = FontStyles.Italic;
            label.color = SteamInputPalette.MouseLineColor;
            label.alignment = TextAlignmentOptions.Left;

            return go.AddComponent<MouseLineController>();
        }

        public class MouseLineController : MonoBehaviour
        {
        }
    }
}
