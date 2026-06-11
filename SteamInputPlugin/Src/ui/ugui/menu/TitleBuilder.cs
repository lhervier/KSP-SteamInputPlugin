using UnityEngine;
using TMPro;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.styles;
using com.github.lhervier.ksp.shared;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.menu
{
    public class TitleBuilder : IUGUIBuilder<TitleBuilder.TitleController>
    {
        // ======================================
        // Build
        // ======================================

        public TitleController Build()
        {
            var go = new GameObject("Title", typeof(RectTransform));
            
            var text = go.AddComponent<TextMeshProUGUI>();
            text.text = ModLocalization.GetString("SteamInput_zonesMenuTitle").ToUpperInvariant();
            text.font = DefaultPalette.Font;
            text.fontSize = SteamInputPalette.MenuTitleFontSize;
            text.fontStyle = FontStyles.Bold;
            text.color = SteamInputPalette.MenuTitleColor;
            text.alignment = TextAlignmentOptions.Left;
            text.enableWordWrapping = false;
            text.overflowMode = TextOverflowModes.Overflow;
            text.raycastTarget = false;

            return go.AddComponent<TitleController>();
        }

        public class TitleController : MonoBehaviour
        {
        }
    }
}