using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using UnityEngine.Events;
using System;

namespace com.github.lhervier.ksp.ui.ugui.menu
{
    public class TitleBuilder
    {
        private CheatSheetViewModel _viewModel;
        
        public TitleBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public GameObject Create()
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

            return go;
        }
    }
}