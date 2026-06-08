using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using com.github.lhervier.ksp.ugui.shared;
using com.github.lhervier.ksp.ugui.shared.styles;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.titleBar
{
    public class TitleBarBuilder : IUGUIBuilder<TitleBarController>
    {
        private CheatSheetViewModel _viewModel;
        private SeparatorBuilder _separatorBuilder;
        private RootBuilder _rootBuilder;

        public TitleBarBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._separatorBuilder = new SeparatorBuilder(viewModel);
            this._rootBuilder = new RootBuilder(viewModel);
        }

        public TitleBarController Create()
        {
            var titleBarGo = new GameObject("SteamInput.TitleBar", typeof(RectTransform));
            TitleBarController controller = titleBarGo.AddComponent<TitleBarController>();

            // If the parent has a layout (and that's the case), forget about me, I will position elements myself.
            var titleBarLayout = titleBarGo.AddComponent<LayoutElement>();
            titleBarLayout.ignoreLayout = true;

            // Title bar zone relative to the parent, stretched horizontaly
            // Beware not to overlap the borders
            var titleBarRect = titleBarGo.GetComponent<RectTransform>();
            titleBarRect.anchorMin = new Vector2(0f, 1f);
            titleBarRect.anchorMax = new Vector2(1f, 1f);
            titleBarRect.pivot = new Vector2(0.5f, 1f);
            titleBarRect.sizeDelta = new Vector2(-2f * PopupPalette.WindowBorderThickness, PopupPalette.TitleBarHeight);
            titleBarRect.anchoredPosition = new Vector2(0f, -PopupPalette.WindowBorderThickness);

            // Image for the backgroup of the title bar
            var headerImage = titleBarGo.AddComponent<Image>();
            headerImage.sprite = SpritesGlobal.FillSprite;
            headerImage.type = Image.Type.Simple;
            headerImage.color = PopupPalette.TitleBarBackgroundColor;
            headerImage.raycastTarget = false;

            // The main part of the title with all the elements
            RootBuilder.RootController rootGo = this._rootBuilder.Create();
            rootGo.transform.SetParent(titleBarGo.transform, false);            

            // The separator
            SeparatorBuilder.SeparatorController separatorGo = _separatorBuilder.Create();
            separatorGo.transform.SetParent(titleBarGo.transform, false);

            return controller;
        }   
    }
}