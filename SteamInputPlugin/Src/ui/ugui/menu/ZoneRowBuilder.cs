using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;

namespace com.github.lhervier.ksp.ui.ugui.menu
{
    public class ZoneRowBuilder
    {
        
        private const float ArrowSpacing = 2f;

        private CheatSheetViewModel _viewModel;
        private CheckboxBuilder _checkBoxBuilder;
        private ZoneLabelBuilder _zoneLabelBuilder;
        private ArrowsBuilder _arrowsBuilder;

        public ZoneRowBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._checkBoxBuilder = new CheckboxBuilder(viewModel);
            this._zoneLabelBuilder = new ZoneLabelBuilder(viewModel);
            this._arrowsBuilder = new ArrowsBuilder(viewModel);
        }

        public GameObject Create()
        {
            var rowGo = new GameObject("Zone", typeof(RectTransform));

            // Horizontal: checkbox + label (greedy) + arrows
            var layout = rowGo.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = SteamInputPalette.DefaultSpacing;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            this._checkBoxBuilder.Create().transform.SetParent(rowGo.transform, false);
            this._zoneLabelBuilder.Create().transform.SetParent(rowGo.transform, false);
            this._arrowsBuilder.Create().transform.SetParent(rowGo.transform, false);

            return rowGo;
        }
    }
}
