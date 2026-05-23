using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using UnityEngine.Events;
using System;

namespace com.github.lhervier.ksp.ui.ugui
{
    public class MenuBuilder
    {
        private CheatSheetViewModel _viewModel;
        
        public MenuBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public GameObject Create()
        {
            var menuGo = new GameObject("SteamInput.TitleBar.Menu", typeof(RectTransform));

            // popupWindow has a VerticalLayoutGroup that would otherwise place us in its flow.
            // Tell it to ignore us so our anchors take effect.
            var layoutElement = menuGo.AddComponent<LayoutElement>();
            layoutElement.ignoreLayout = true;

            // Anchored at the popup's top-right corner. The offset aligns the menu's top-right
            // with the close/menu button's bottom-right (drop-down-from-the-button look):
            //   X: WindowBorderThickness (title bar inset) + DefaultPaddingRight (HLG right padding)
            //   Y: WindowBorderThickness (title bar shift) + TitleBarHeight - DefaultPaddingBottom
            //      (= the button's bottom inside the title bar)
            // Width is fixed; height is left to the ContentSizeFitter below.
            var rect = menuGo.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.sizeDelta = new Vector2(SteamInputPalette.MenuWidth, 0f);
            rect.anchoredPosition = new Vector2(
                -(SteamInputPalette.WindowBorderThickness + SteamInputPalette.DefaultPaddingRight),
                -(SteamInputPalette.WindowBorderThickness + SteamInputPalette.TitleBarHeight - SteamInputPalette.DefaultPaddingBottom)
            );

            // Sliced chrome: dark background with a 1px border
            var image = menuGo.AddComponent<Image>();
            image.sprite = SpritesZonesMenu.ChromeSprite;
            image.type = Image.Type.Sliced;
            image.color = Color.white;
            // Catches clicks so they don't fall through to the overlay (which would close the menu)
            image.raycastTarget = true;

            // Vertical layout for the menu content (title / separator / items).
            // Padding is on the menu itself (Option A) — every row inherits the same horizontal inset.
            var layout = menuGo.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(
                Mathf.RoundToInt(SteamInputPalette.MenuPaddingLeft), 
                Mathf.RoundToInt(SteamInputPalette.MenuPaddingRight),
                Mathf.RoundToInt(SteamInputPalette.MenuPaddingTop), 
                Mathf.RoundToInt(SteamInputPalette.MenuPaddingBottom)
            );
            layout.spacing = SteamInputPalette.MenuSpacing;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            // Auto-grow the menu height to fit its content. Width stays fixed at MenuWidth.
            var fitter = menuGo.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            CreateTitle().transform.SetParent(menuGo.transform, false);
            CreateSeparator().transform.SetParent(menuGo.transform, false);
            CreateZonesPlaceholder().transform.SetParent(menuGo.transform, false);

            return menuGo;
        }

        private GameObject CreateTitle()
        {
            var go = new GameObject("Title", typeof(RectTransform));

            var text = go.AddComponent<Text>();
            text.text = ModLocalization.GetString("LOC_SteamInput_zonesMenuTitle").ToUpperInvariant();
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

        private GameObject CreateSeparator()
        {
            var go = new GameObject("Separator", typeof(RectTransform));

            // 1px tall, full width (the parent VLG stretches it via childForceExpandWidth = true)
            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 1f;
            layoutElement.minHeight = 1f;

            var image = go.AddComponent<Image>();
            image.sprite = SpritesGlobal.FillSprite;
            image.type = Image.Type.Simple;
            image.color = SteamInputPalette.MenuSeparatorColor;
            image.raycastTarget = false;

            return go;
        }

        private GameObject CreateZonesPlaceholder()
        {
            var go = new GameObject("Zones", typeof(RectTransform));

            var text = go.AddComponent<Text>();
            text.text = "<Zones>";
            text.font = HighLogic.UISkin.font;
            text.fontSize = 10;
            text.color = SteamInputPalette.DefaultButtonTextColor;
            text.alignment = TextAnchor.MiddleLeft;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;

            return go;
        }
    }
}