using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using UnityEngine.Events;
using System;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.menu
{
    public class TitleBuilder
    {
        private CheatSheetViewModel _viewModel;
        
        public TitleBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public TitleController Create()
        {
            var go = new GameObject("Title", typeof(RectTransform));
            TitleController controller = go.AddComponent<TitleController>();
            controller.ViewModel(_viewModel);

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

            return controller;
        }

        public class TitleController : BaseSteamInputController
        {
        }
    }
}