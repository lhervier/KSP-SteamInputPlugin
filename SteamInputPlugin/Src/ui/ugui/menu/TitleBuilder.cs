using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.shared.ugui;
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
            
            var text = go.AddComponent<Text>();
            text.text = ModLocalization.GetString("SteamInput_zonesMenuTitle").ToUpperInvariant();
            text.font = HighLogic.UISkin.font;
            text.fontSize = 10;
            text.fontStyle = FontStyle.Bold;
            text.color = SteamInputPalette.MenuTitleColor;
            text.alignment = TextAnchor.MiddleLeft;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;

            return go.AddComponent<TitleController>();
        }

        public class TitleController : MonoBehaviour
        {
        }
    }
}