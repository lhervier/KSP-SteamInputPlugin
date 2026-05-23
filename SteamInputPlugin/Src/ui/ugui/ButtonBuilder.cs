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
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.1f;
            button.colors = colors;
            button.interactable = interactable;
            button.onClick.AddListener(() => onClick());

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
            if( interactable )
            {
                label.color = SteamInputPalette.DefaultButtonTextColor;
            }
            else
            {
                label.color = SteamInputPalette.DefaultButtonDisabledTextColor;
            }

            label.alignment = TextAnchor.MiddleCenter;
            label.raycastTarget = false;

            ButtonController controller = buttonGo.AddComponent<ButtonController>();
            controller.Initialize(label, button);

            // Button.colors only tints the targetGraphic (the background); replicate IMGUI's text
            // color swap (ButtonText → white on hover) via an EventTrigger on the same GameObject.
            var trigger = buttonGo.AddComponent<EventTrigger>();
            var enterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enterEntry.callback.AddListener(_ => {
                if( !button.interactable ) return;
                label.color = Color.white;
            });
            trigger.triggers.Add(enterEntry);
            var exitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            exitEntry.callback.AddListener(_ => {
                if( !button.interactable ) return;
                label.color = SteamInputPalette.DefaultButtonTextColor;
            });
            trigger.triggers.Add(exitEntry);

            return controller;
        }
    }

    public class ButtonController : MonoBehaviour
    {
        private Text _label;
        private Button _button;

        public void Initialize(Text label, Button button)
        {
            this._label = label;
            this._button = button;
        }

        public bool IsInteractable()
        {
            return _button.interactable;
        }

        public void SetInteractable(bool enableState)
        {
            _button.interactable = enableState;
            if( enableState )
            {
                _label.color = SteamInputPalette.DefaultButtonTextColor;
            }
            else
            {
                _label.color = SteamInputPalette.DefaultButtonDisabledTextColor;
            }
        }
    }
}