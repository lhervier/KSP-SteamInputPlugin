using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;

namespace com.github.lhervier.ksp.ui.ugui.menu
{
    public class ArrowsBuilder
    {
        private CheatSheetViewModel _viewModel;
        private ArrowButtonBuilder _arrowBuilder;

        public ArrowsBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._arrowBuilder = new ArrowButtonBuilder(viewModel);
        }

        public GameObject Create()
        {
            var go = new GameObject("Arrows", typeof(RectTransform));

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = SteamInputPalette.MenuArrowsSpacing;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            this._arrowBuilder.Create("Up", "▲", () => Debug.Log("[SteamInput] Zone UP"))
                .transform.SetParent(go.transform, false);
            this._arrowBuilder.Create("Down", "▼", () => Debug.Log("[SteamInput] Zone DOWN"))
                .transform.SetParent(go.transform, false);

            return go;
        }
    }
}
