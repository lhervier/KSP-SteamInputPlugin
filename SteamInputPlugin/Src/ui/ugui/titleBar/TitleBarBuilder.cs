using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using UnityEngine.Events;
using System;

namespace com.github.lhervier.ksp.ui.ugui.titleBar
{
    public class TitleBarBuilder
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

        public TitleBarController Create(Action toggleMenu)
        {
            var titleBarGo = new GameObject("SteamInput.TitleBar", typeof(RectTransform));
            TitleBarController controller = titleBarGo.AddComponent<TitleBarController>();
            controller.Initialize(_viewModel);

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

            // The main part of the title with all the elements
            RootBuilder.RootController rootGo = this._rootBuilder.Create(toggleMenu);
            rootGo.transform.SetParent(titleBarGo.transform, false);            

            // The separator
            SeparatorBuilder.SeparatorController separatorGo = _separatorBuilder.Create();
            separatorGo.transform.SetParent(titleBarGo.transform, false);

            return controller;
        }

        public class TitleBarController : BaseSteamInputController
        {
        }
    }
}