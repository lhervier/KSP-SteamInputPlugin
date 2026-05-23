using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using UnityEngine.Events;
using System;

namespace com.github.lhervier.ksp.ui.ugui
{
    public class ButtonBuilder
    {
        private CheatSheetViewModel _viewModel;
        public ButtonBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public ButtonController Create(
            string objectName, 
            string buttonLabel,
            Action onClick,
            bool interactable = true
        )
        {
            return Create(
                objectName, 
                buttonLabel,
                onClick, 
                interactable,
                SteamInputPalette.DefaultButtonColor, 
                SteamInputPalette.DefaultButtonHoverColor
            );
        }

        public ButtonController Create(
            string objectName, 
            string buttonLabel,
            Action onClick,
            bool interactable,
            Color backgroundColor,
            Color hoverColor
        )
        {
            var buttonGo = new GameObject(objectName, typeof(RectTransform));
            ButtonController controller = buttonGo.AddComponent<ButtonController>();
            controller.Initialize(_viewModel);

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
            colors.normalColor = backgroundColor;
            colors.highlightedColor = hoverColor;
            colors.pressedColor = backgroundColor;
            colors.selectedColor = backgroundColor;
            // Suppress Unity's default disabled gray tint; the CanvasGroup below does the fade.
            colors.disabledColor = backgroundColor;
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.1f;
            button.colors = colors;
            button.onClick.AddListener(() => onClick());
            controller.InitButton(button);

            // CanvasGroup applies a global alpha to the button (background + text + future children),
            // and also blocks raycasts when disabled — matches the mockup's .ka:disabled { opacity: .25 }.
            var canvasGroup = buttonGo.AddComponent<CanvasGroup>();
            controller.InitCanvasGroup(canvasGroup);

            // Button label, centered in the button
            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(buttonGo.transform, false);
            var labelRect = labelGo.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            var label = labelGo.AddComponent<Text>();
            label.text = buttonLabel;
            label.font = HighLogic.UISkin.font;
            label.fontSize = 13;
            label.color = SteamInputPalette.DefaultButtonTextColor;
            label.alignment = TextAnchor.MiddleCenter;
            label.raycastTarget = false;
            controller.InitLabel(label);

            // EventTrigger swaps the label color to white on hover. When disabled, the CanvasGroup
            // sets blocksRaycasts = false so these never fire — no need for an extra interactable check.
            var trigger = buttonGo.AddComponent<EventTrigger>();
            var enterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enterEntry.callback.AddListener(_ => label.color = Color.white);
            trigger.triggers.Add(enterEntry);
            var exitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            exitEntry.callback.AddListener(_ => label.color = SteamInputPalette.DefaultButtonTextColor);
            trigger.triggers.Add(exitEntry);

            // Apply the initial interactable state via the controller (single source of truth)
            controller.SetInteractable(interactable);

            return controller;
        }
    }

    public class ButtonController : BaseSteamInputController
    {
        private const float DisabledAlpha = 0.25f;

        private Text _label;
        private Button _button;
        private CanvasGroup _canvasGroup;

        public void InitLabel(Text label)
        {
            this._label = label;
        }

        public void InitButton(Button button)
        {
            this._button = button;
        }

        public void InitCanvasGroup(CanvasGroup canvasGroup)
        {
            this._canvasGroup = canvasGroup;
        }

        public bool IsInteractable()
        {
            return _button != null && _button.interactable;
        }

        public void SetInteractable(bool interactable)
        {
            if (_button != null) _button.interactable = interactable;
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = interactable ? 1f : DisabledAlpha;
                _canvasGroup.blocksRaycasts = interactable;
                _canvasGroup.interactable = interactable;
            }
            // Reset to default in case the label was left in the "hover white" state when disabled.
            if (_label != null) _label.color = SteamInputPalette.DefaultButtonTextColor;
        }
    }
}