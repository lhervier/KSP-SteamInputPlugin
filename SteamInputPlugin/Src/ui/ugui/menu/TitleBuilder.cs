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
            
            var text = UGUILabels.AddLabel(go);
            text.text = ModLocalization.GetString("SteamInput_zonesMenuTitle").ToUpperInvariant();
            text.fontSize = SteamInputPalette.MenuTitleFontSize;
            text.fontStyle = FontStyles.Bold;
            text.color = SteamInputPalette.MenuTitleColor;
            text.alignment = TextAlignmentOptions.Left;

            return go.AddComponent<TitleController>();
        }

        public class TitleController : MonoBehaviour
        {
        }
    }
}