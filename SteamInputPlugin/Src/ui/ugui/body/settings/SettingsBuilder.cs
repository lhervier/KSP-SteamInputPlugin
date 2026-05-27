using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.model;
using System.Collections.Generic;

namespace com.github.lhervier.ksp.ui.ugui.body.settings
{
    /// <summary>
    /// Container that stacks one PhysicalZone per UIPhysicalZone from the ViewModel.
    /// Only visible zones are rendered (Visible flag from the toggle menu).
    /// </summary>
    public class SettingsBuilder
    {
        private CheatSheetViewModel _viewModel;
        
        public SettingsBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public SettingsController Create()
        {
            var go = new GameObject("PhysicalZones", typeof(RectTransform));
            var controller = go.AddComponent<SettingsController>();
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

            return controller;
        }

        public class SettingsController : BaseSteamInputController
        {
        }
    }
}
