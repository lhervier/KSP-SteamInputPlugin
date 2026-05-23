using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using System;

namespace com.github.lhervier.ksp.ui.ugui.menu
{
    public class ArrowButtonBuilder
    {
        private CheatSheetViewModel _viewModel;
        
        public ArrowButtonBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public GameObject Create(string name, string label, Action onClick)
        {
            var buttonGo = new GameObject(name, typeof(RectTransform));

            var layoutElement = buttonGo.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = SteamInputPalette.DefaultButtonSize;
            layoutElement.preferredHeight = SteamInputPalette.DefaultButtonSize;
            layoutElement.minWidth = SteamInputPalette.DefaultButtonSize;
            layoutElement.minHeight = SteamInputPalette.DefaultButtonSize;

            // White background so the Button's color tint applies as-is
            var image = buttonGo.AddComponent<Image>();
            image.sprite = SpritesGlobal.FillSprite;
            image.type = Image.Type.Simple;
            image.color = Color.white;
            image.raycastTarget = true;

            var button = buttonGo.AddComponent<Button>();
            button.targetGraphic = image;
            var colors = button.colors;
            colors.normalColor = SteamInputPalette.DefaultButtonColor;
            colors.highlightedColor = SteamInputPalette.DefaultButtonHoverColor;
            colors.pressedColor = SteamInputPalette.DefaultButtonColor;
            colors.selectedColor = SteamInputPalette.DefaultButtonColor;
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.1f;
            button.colors = colors;
            button.onClick.AddListener(() => onClick());

            // Arrow glyph, centered in the button
            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(buttonGo.transform, false);
            var labelRect = labelGo.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            var text = labelGo.AddComponent<Text>();
            text.text = label;
            text.font = HighLogic.UISkin.font;
            text.fontSize = 10;
            text.color = SteamInputPalette.DefaultButtonTextColor;
            text.alignment = TextAnchor.MiddleCenter;
            text.raycastTarget = false;

            return buttonGo;
        }
    }
}
