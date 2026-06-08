using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using UnityEngine.Events;
using System;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.titleBar
{
    public class RootBuilder
    {
        private CheatSheetViewModel _viewModel;
        private LeftColumnBuilder _leftColumnBuilder;
        private RightColumnBuilder _rightColumnBuiler;

        public RootBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._leftColumnBuilder = new LeftColumnBuilder(viewModel);
            this._rightColumnBuiler = new RightColumnBuilder(viewModel);
        }

        public RootController Create()
        {
            var rootGo = new GameObject("SteamInput.TitleBar.Root", typeof(RectTransform));
            RootController controller = rootGo.AddComponent<RootController>();
            controller.Initialize(this._viewModel);
            
            // Full size of the parent = the title bar, minus the bottom separator
            var rootRect = rootGo.GetComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = new Vector2(0f, SteamInputPalette.TitleBarSeparatorHeight);
            rootRect.offsetMax = Vector2.zero;

            // Horizontal layout splitting the title bar in two cells (left + right)
            var rootLayout = rootGo.AddComponent<HorizontalLayoutGroup>();
            rootLayout.padding = new RectOffset(
                Mathf.RoundToInt(DefaultPalette.PaddingLeft),
                Mathf.RoundToInt(DefaultPalette.PaddingRight),
                Mathf.RoundToInt(DefaultPalette.PaddingTop),
                Mathf.RoundToInt(DefaultPalette.PaddingBottom)
            );
            rootLayout.spacing = 0f;
            rootLayout.childAlignment = TextAnchor.MiddleLeft;
            // Width controlled by the layout so flexibleWidth on LeftRow pushes RightRow to the right
            // Height forced so the rows fill the title bar's height for proper vertical centering
            rootLayout.childControlWidth = true;
            rootLayout.childControlHeight = true;
            rootLayout.childForceExpandWidth = false;
            rootLayout.childForceExpandHeight = true;

            var leftRow = this._leftColumnBuilder.Create();
            leftRow.transform.SetParent(rootGo.transform, false);

            var rightRow = this._rightColumnBuiler.Create();
            rightRow.transform.SetParent(rootGo.transform, false);

            return controller;
        }

        public class RootController : BaseSteamInputController
        {
        }
    }
}