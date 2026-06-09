using UnityEngine;
using com.github.lhervier.ksp.steaminput.ui.ugui.body.cheatsheet;
using com.github.lhervier.ksp.steaminput.ui.ugui.body.settings;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.scrollableview;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body
{
    /// <summary>
    /// Body of the popup (below the title bar). Hosts the two switchable views — the scrollable cheat
    /// sheet (config selector + zones) and the non-scrollable settings screen — both filling the body;
    /// the controller shows one at a time.
    /// </summary>
    public class BodyBuilder : IUGUIBuilder<BodyController>
    {
        private CheatSheetViewModel _viewModel;
        private CheatSheetBuilder _cheatSheetBuilder;
        private SettingsBuilder _settingsBuilder;

        public BodyBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._cheatSheetBuilder = new CheatSheetBuilder(viewModel);
            this._settingsBuilder = new SettingsBuilder(viewModel);
        }

        public BodyController Build()
        {
            var bodyGo = new GameObject("SteamInput.Body", typeof(RectTransform));
            var controller = bodyGo.AddComponent<BodyController>();

            // Scrollable cheat-sheet view; ScrollableView's default styling matches the popup palette.
            var cheatSheetController = new ScrollableViewBuilder<CheatSheetBuilder.CheatSheetController>()
                .ObjectName("SteamInput.Body.CheatSheet")
                .ContentBuilder(_cheatSheetBuilder)
                .Build();
            FillParent(cheatSheetController.gameObject, bodyGo.transform);

            // Settings view, not scrollable.
            var settingsController = _settingsBuilder.Create();
            FillParent(settingsController.gameObject, bodyGo.transform);

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
    }
}
