using UnityEngine;
using com.github.lhervier.ksp.steaminput.ui.ugui.menu;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.overlay;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.overlays
{
    /// <summary>
    /// Builds the popup overlays (the drop-down menu and the click trap behind it) on a single root.
    /// The PopupBuilder grafts this root over the whole window, above the content and the title bar; the
    /// returned controller self-manages their visibility through the ViewModel.
    /// </summary>
    public class SteamInputOverlaysBuilder : IUGUIBuilder<SteamInputOverlaysController>
    {
        private CheatSheetViewModel _viewModel;
        public SteamInputOverlaysBuilder WithViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        public SteamInputOverlaysController Build()
        {
            // Just assemble the overlays on a root; the PopupBuilder parents it onto the window and
            // stretches it full-window (above the content and the title bar). Each overlay self-anchors here.
            var rootGo = new GameObject("SteamInput.Overlays", typeof(RectTransform));

            // Full-window click trap behind the menu: a click outside the menu closes it.
            OverlayController overlayController = new OverlayBuilder().Build();
            overlayController.transform.SetParent(rootGo.transform, false);
            overlayController.gameObject.SetActive(false);

            // The drop-down menu itself, anchored top-right under the title bar.
            MenuBuilder.MenuController menuController = new MenuBuilder()
                .WithViewModel(_viewModel)
                .Build();
            menuController.transform.SetParent(rootGo.transform, false);
            menuController.gameObject.SetActive(false);

            return rootGo
                .AddComponent<SteamInputOverlaysController>()
                .WithViewModel(_viewModel)
                .WithOverlayController(overlayController)
                .WithMenuController(menuController);
        }
    }
}
