using UnityEngine;
using UnityEngine.UI;
using System;
using com.github.lhervier.ksp.ugui.shared.sprites;

namespace com.github.lhervier.ksp.steaminput.ui.ugui
{
    public class OverlayBuilder
    {
        private CheatSheetViewModel _viewModel;
        
        public OverlayBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public OverlayController Create(Action onClose)
        {
            var overlayGo = new GameObject("SteamInput.Overlay", typeof(RectTransform));
            OverlayController controller = overlayGo.AddComponent<OverlayController>();
            controller.Initialize(_viewModel);

            // popupWindow has a VerticalLayoutGroup that would otherwise place us in its flow.
            // Tell it to ignore us so our anchors take effect.
            var layoutElement = overlayGo.AddComponent<LayoutElement>();
            layoutElement.ignoreLayout = true;

            // Centered on the popup, oversized so it covers anything around the screen
            var rect = overlayGo.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(3000f, 3000f);
            rect.anchoredPosition = Vector2.zero;

            // Fully transparent but raycastTarget=true: invisible click trap
            var image = overlayGo.AddComponent<Image>();
            image.sprite = SpritesGlobal.FillSprite;
            image.type = Image.Type.Simple;
            image.color = new Color(0f, 0f, 0f, 0f);
            image.raycastTarget = true;

            // A click anywhere on the overlay closes the menu. Disable color transitions so the
            // overlay stays invisible during hover/press states.
            var button = overlayGo.AddComponent<Button>();
            button.targetGraphic = image;
            var colors = button.colors;
            colors.normalColor = new Color(1f, 1f, 1f, 0f);
            colors.highlightedColor = new Color(1f, 1f, 1f, 0f);
            colors.pressedColor = new Color(1f, 1f, 1f, 0f);
            colors.selectedColor = new Color(1f, 1f, 1f, 0f);
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0f;
            button.colors = colors;
            button.onClick.AddListener(() => onClose());

            return controller;
        }

        public class OverlayController : BaseSteamInputController
        {
        }
    }
}