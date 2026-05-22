using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using UnityEngine.Events;

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
            var rect = menuGo.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.sizeDelta = new Vector2(SteamInputPalette.MenuWidth, SteamInputPalette.MenuHeight);
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

            return menuGo;
        }
    }
}