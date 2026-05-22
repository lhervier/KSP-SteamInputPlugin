using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using UnityEngine.Events;

namespace com.github.lhervier.ksp.ui.ugui.titleBar
{
    public class MenuButtonBuilder
    {
        private CheatSheetViewModel _viewModel;
        
        public MenuButtonBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public GameObject Create(UnityAction onClick)
        {
            var buttonGo = new GameObject("SteamInput.TitleBar.RightColumn.MenuButton", typeof(RectTransform));

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

            // Button: same color states as CloseButton (matches IMGUI's MenuButton which inherits CloseButton)
            var button = buttonGo.AddComponent<Button>();
            button.targetGraphic = image;
            var colors = button.colors;
            colors.normalColor = SteamInputPalette.Button;
            colors.highlightedColor = SteamInputPalette.ButtonHover;
            colors.pressedColor = SteamInputPalette.Button;
            colors.selectedColor = SteamInputPalette.Button;
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.1f;
            button.colors = colors;
            button.onClick.AddListener(onClick);

            // The "⋯" label (U+22EF), centered in the button
            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(buttonGo.transform, false);
            var labelRect = labelGo.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            var label = labelGo.AddComponent<Text>();
            label.text = "⋯";
            label.font = HighLogic.UISkin.font;
            label.fontSize = 13;
            label.color = SteamInputPalette.ButtonText;
            label.alignment = TextAnchor.MiddleCenter;
            label.raycastTarget = false;

            // Same text-color hover swap as CloseButton (ButtonText → white)
            var trigger = buttonGo.AddComponent<EventTrigger>();
            var enterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enterEntry.callback.AddListener(_ => label.color = Color.white);
            trigger.triggers.Add(enterEntry);
            var exitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            exitEntry.callback.AddListener(_ => label.color = SteamInputPalette.ButtonText);
            trigger.triggers.Add(exitEntry);

            return buttonGo;
        }
    }
}