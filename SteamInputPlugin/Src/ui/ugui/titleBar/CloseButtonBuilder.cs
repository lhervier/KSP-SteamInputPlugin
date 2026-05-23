using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using UnityEngine.Events;

namespace com.github.lhervier.ksp.ui.ugui.titleBar
{
    public class CloseButtonBuilder
    {
        private CheatSheetViewModel _viewModel;
        public CloseButtonBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public GameObject Create()
        {
            var buttonGo = new GameObject("SteamInput.TitleBar.RightColumn.Close", typeof(RectTransform));

            // Fixed square size; parent's HorizontalLayoutGroup has childControl* = true so it reads these
            var layoutElement = buttonGo.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = SteamInputPalette.DefaultButtonSize;
            layoutElement.preferredHeight = SteamInputPalette.DefaultButtonSize;
            layoutElement.minWidth = SteamInputPalette.DefaultButtonSize;
            layoutElement.minHeight = SteamInputPalette.DefaultButtonSize;

            // White background fill so the Button's color tint applies as-is (no multiplication)
            var image = buttonGo.AddComponent<Image>();
            image.sprite = SpritesGlobal.FillSprite;
            image.type = Image.Type.Simple;
            image.color = Color.white;
            image.raycastTarget = true;

            // Button: hover/press color transitions on the background, plus the click handler
            var button = buttonGo.AddComponent<Button>();
            button.targetGraphic = image;
            var colors = button.colors;
            colors.normalColor = SteamInputPalette.TitleBarButtonColor;
            colors.highlightedColor = SteamInputPalette.TitleBarButtonHoverColor;
            colors.pressedColor = SteamInputPalette.TitleBarButtonColor;
            colors.selectedColor = SteamInputPalette.TitleBarButtonColor;
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.1f;
            button.colors = colors;
            button.onClick.AddListener(() => this._viewModel.CloseWindow());

            // The "X" label, centered in the button
            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(buttonGo.transform, false);
            var labelRect = labelGo.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            var label = labelGo.AddComponent<Text>();
            label.text = "×";
            label.font = HighLogic.UISkin.font;
            label.fontSize = 13;
            label.color = SteamInputPalette.DefaultButtonTextColor;
            label.alignment = TextAnchor.MiddleCenter;
            label.raycastTarget = false;

            // Button.colors only tints the targetGraphic (the background); replicate IMGUI's text
            // color swap (ButtonText → white on hover) via an EventTrigger on the same GameObject.
            var trigger = buttonGo.AddComponent<EventTrigger>();
            var enterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enterEntry.callback.AddListener(_ => label.color = Color.white);
            trigger.triggers.Add(enterEntry);
            var exitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            exitEntry.callback.AddListener(_ => label.color = SteamInputPalette.DefaultButtonTextColor);
            trigger.triggers.Add(exitEntry);

            return buttonGo;
        }
    }
}