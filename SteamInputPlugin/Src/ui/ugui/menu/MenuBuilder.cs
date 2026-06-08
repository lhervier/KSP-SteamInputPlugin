using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using UnityEngine.Events;
using System;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.menu
{
    public class MenuBuilder
    {
        private CheatSheetViewModel _viewModel;
        private TitleBuilder _titleBuilder;
        private SeparatorBuilder _separatorBuilder;
        private ZonesBuilder _zonesBuilder;
        private SettingsItemBuilder _settingsItemBuilder;

        public MenuBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._titleBuilder = new TitleBuilder(viewModel);
            this._separatorBuilder = new SeparatorBuilder(viewModel);
            this._zonesBuilder = new ZonesBuilder(viewModel);
            this._settingsItemBuilder = new SettingsItemBuilder(viewModel);
        }

        public MenuController Create()
        {
            var menuGo = new GameObject("SteamInput.TitleBar.Menu", typeof(RectTransform));
            MenuController controller = menuGo.AddComponent<MenuController>();
            controller.Initialize(_viewModel);

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
                -(SteamInputPalette.WindowBorderThickness + DefaultPalette.PaddingRight),
                -(SteamInputPalette.WindowBorderThickness + SteamInputPalette.TitleBarHeight - DefaultPalette.PaddingBottom)
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

            // Title / separator / zone rows, then a separator and the "Settings" entry.
            _titleBuilder.Create().transform.SetParent(menuGo.transform, false);
            _separatorBuilder.Create().transform.SetParent(menuGo.transform, false);
            _zonesBuilder.Create().transform.SetParent(menuGo.transform, false);
            _separatorBuilder.Create().transform.SetParent(menuGo.transform, false);
            _settingsItemBuilder.Create().transform.SetParent(menuGo.transform, false);

            return controller;
        }

        public class MenuController : BaseSteamInputController
        {
        }
    }
}