using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using UnityEngine.Events;

namespace com.github.lhervier.ksp.ui.ugui.titleBar
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

        public GameObject Create(UnityAction onMenuToggle)
        {
            var rootGo = new GameObject("SteamInput.TitleBar.Root", typeof(RectTransform));

            // Full size of the parent = the title bar, minus the bottom separator
            var rootRect = rootGo.GetComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = new Vector2(0f, SteamInputPalette.TitleBarSeparatorHeight);
            rootRect.offsetMax = Vector2.zero;

            // Horizontal layout splitting the title bar in two cells (left + right)
            var rootLayout = rootGo.AddComponent<HorizontalLayoutGroup>();
            rootLayout.padding = new RectOffset(
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingLeft),
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingRight),
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingTop),
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingBottom)
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

            var rightRow = this._rightColumnBuiler.Create(onMenuToggle);
            rightRow.transform.SetParent(rootGo.transform, false);

            return rootGo;
        }
    }
}