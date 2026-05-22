using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using UnityEngine.Events;

namespace com.github.lhervier.ksp.ui.ugui.titleBar
{
    public class TitleBarBuilder
    {
        private CheatSheetViewModel _viewModel;
        private SeparatorBuilder _separatorBuilder;
        private OverlayBuilder _overlayBuilder;
        private RootBuilder _rootBuilder;

        public TitleBarBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._separatorBuilder = new SeparatorBuilder(viewModel);
            this._overlayBuilder = new OverlayBuilder(viewModel);
            this._rootBuilder = new RootBuilder(viewModel);
        }

        public GameObject Create()
        {
            var titleBarGo = new GameObject("SteamInput.TitleBar", typeof(RectTransform));
            
            // If the parent has a layout (and that's the case), forget about me, I will position elements myself.
            var titleBarLayout = titleBarGo.AddComponent<LayoutElement>();
            titleBarLayout.ignoreLayout = true;

            // Title bar zone relative to the parent, stretched horizontaly
            // Beware not to overlap the borders
            var titleBarRect = titleBarGo.GetComponent<RectTransform>();
            titleBarRect.anchorMin = new Vector2(0f, 1f);
            titleBarRect.anchorMax = new Vector2(1f, 1f);
            titleBarRect.pivot = new Vector2(0.5f, 1f);
            titleBarRect.sizeDelta = new Vector2(-2f * SteamInputPalette.WindowBorderThickness, SteamInputPalette.TitleBarHeight);
            titleBarRect.anchoredPosition = new Vector2(0f, -SteamInputPalette.WindowBorderThickness);

            // Image for the backgroup of the title bar
            var headerImage = titleBarGo.AddComponent<Image>();
            headerImage.sprite = SpritesGlobal.FillSprite;
            headerImage.type = Image.Type.Simple;
            headerImage.color = SteamInputPalette.Header;
            headerImage.raycastTarget = false;

            GameObject separatorGo = _separatorBuilder.Create();
            separatorGo.transform.SetParent(titleBarGo.transform, false);

            // Menu and overlay are created BEFORE the root so the callbacks below can capture them.
            // They are parented LATER (after the root) to control sibling order = z-order:
            //   separator (bottom) → root → overlay → menu (top).
            GameObject menuGo = null;
            GameObject overlayGo = null;
            UnityAction toggleMenu = () => {
                bool willOpen = !menuGo.activeSelf;
                menuGo.SetActive(willOpen);
                overlayGo.SetActive(willOpen);
            };
            UnityAction closeMenu = () => {
                menuGo.SetActive(false);
                overlayGo.SetActive(false);
            };

            GameObject rootGo = this._rootBuilder.Create(toggleMenu);
            rootGo.transform.SetParent(titleBarGo.transform, false);

            overlayGo = _overlayBuilder.Create(closeMenu);
            overlayGo.transform.SetParent(titleBarGo.transform, false);
            overlayGo.SetActive(false);

            menuGo = CreateMenu();
            menuGo.transform.SetParent(titleBarGo.transform, false);
            menuGo.SetActive(false);

            return titleBarGo;
        }

        private GameObject CreateMenu()
        {
            var menuGo = new GameObject("SteamInput.TitleBar.Menu", typeof(RectTransform));

            // Anchored at the title bar's bottom-right corner; offset inward by the title bar's
            // padding so the menu's top-right corner aligns with the menu button's bottom-right
            // (gives the "dropdown coming from the button" look from the mockup).
            var rect = menuGo.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(1f, 1f);
            rect.sizeDelta = new Vector2(ZonesMenuStyles.PanelWidth, ZonesMenuStyles.ContentPlaceholderHeight);
            rect.anchoredPosition = new Vector2(
                -SteamInputPalette.DefaultPaddingRight,
                SteamInputPalette.DefaultPaddingBottom
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