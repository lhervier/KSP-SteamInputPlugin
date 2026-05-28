using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.model;
using System.Collections.Generic;

namespace com.github.lhervier.ksp.ui.ugui.body.zones
{
    /// <summary>
    /// Container that stacks one PhysicalZone per UIPhysicalZone from the ViewModel.
    /// Only visible zones are rendered (Visible flag from the toggle menu).
    /// </summary>
    public class EmptyConfigBuilder
    {
        private CheatSheetViewModel _viewModel;

        public EmptyConfigBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public EmptyConfigController Create()
        {
            var go = new GameObject("EmptyConfig", typeof(RectTransform));
            var controller = go.AddComponent<EmptyConfigController>();
            controller.Initialize(_viewModel);

            // VLG stacks the zones back-to-back. No spacing — each zone has its own bottom separator.
            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = 0f;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            // Placeholder message
            var labelGo = new GameObject("EmptyMessage", typeof(RectTransform));
            labelGo.transform.SetParent(go.transform, false);

            var label = labelGo.AddComponent<Text>();
            label.text = "<No config selected>";
            label.font = HighLogic.UISkin.font;
            label.fontSize = 12;
            label.fontStyle = FontStyle.Bold;
            label.alignment = TextAnchor.MiddleLeft;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;

            return controller;
        }

        public class EmptyConfigController : BaseSteamInputController
        {
        }
    }
}
