using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.steaminput.ui.ugui.body.cheatsheet;
using com.github.lhervier.ksp.steaminput.ui.ugui.body.settings;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.scrollableview;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body
{
    /// <summary>
    /// Body of the popup (below the title bar). Hosts the two switchable views — the scrollable cheat
    /// sheet (config selector + zones), which fills the body, and the non-scrollable settings screen,
    /// pinned to the top and sized to its content; the controller shows one at a time.
    /// </summary>
    public class BodyBuilder : IUGUIBuilder<BodyController>
    {
        private CheatSheetViewModel _viewModel;

        public BodyBuilder ViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        public BodyController Build()
        {
            var bodyGo = new GameObject("SteamInput.Body", typeof(RectTransform));
            var controller = bodyGo.AddComponent<BodyController>();

            // Scrollable cheat-sheet view; ScrollableView's default styling matches the popup palette.
            var cheatSheetController = new ScrollableViewBuilder<CheatSheetController>()
                .ObjectName("SteamInput.Body.CheatSheet")
                .ContentBuilder(
                    new CheatSheetBuilder().ViewModel(_viewModel)
                )
                .Build();
            FillParent(cheatSheetController.gameObject, bodyGo.transform);

            // Settings view, not scrollable. Top-anchored and sized to its content (it never
            // outgrows the body): stretching it to the full body height would hand the leftover
            // space to whichever row reports a flexible height.
            var settingsController = new SettingsBuilder()
                .ViewModel(_viewModel)
                .Build();
            TopFitParent(settingsController.gameObject, bodyGo.transform);

            return controller
                .ViewModel(_viewModel)
                .CheatSheetController(cheatSheetController)
                .SettingsController(settingsController);
        }

        // Parent the view and stretch it to fill the body.
        private static void FillParent(GameObject go, Transform parent)
        {
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        // Parent the view, stretch it to the body's width and pin it to the body's top;
        // its height follows its preferred (content) height instead of the body's.
        private static void TopFitParent(GameObject go, Transform parent)
        {
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 1f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var fitter = go.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }
}
