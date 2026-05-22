using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using UnityEngine.Events;

namespace com.github.lhervier.ksp.ui.ugui.titleBar
{
    public class OverlayBuilder
    {
        private CheatSheetViewModel _viewModel;
        
        public OverlayBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public GameObject Create(UnityAction onClick)
        {
            var overlayGo = new GameObject("SteamInput.TitleBar.MenuOverlay", typeof(RectTransform));

            // Anchored to the title bar's bottom-center; oversized so it covers anything below the title bar
            // (popup body, and beyond if the popup is small relative to the screen).
            var rect = overlayGo.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 1f);
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
            button.onClick.AddListener(onClick);

            return overlayGo;
        }
    }
}