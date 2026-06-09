using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.steaminput.ui.ugui.body.selector;
using com.github.lhervier.ksp.steaminput.ui.ugui.body.zones;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.cheatsheet
{
    /// <summary>
    /// The cheat-sheet view shown in the scrollable body: the config selector on top, then the list of
    /// physical zones below it. Both scroll together. This is one of the body's two views (the other
    /// being the settings screen).
    /// </summary>
    public class CheatSheetBuilder : IUGUIBuilder<CheatSheetController>
    {
        private CheatSheetViewModel _viewModel;
        public CheatSheetBuilder ViewModel(CheatSheetViewModel viewModel)
        {
            _viewModel = viewModel;
            return this;
        }

        public CheatSheetController Build()
        {
            var go = new GameObject("SteamInput.Body.CheatSheet.Content", typeof(RectTransform));
            var controller = go.AddComponent<CheatSheetController>();

            // Stacks the selector header above the zone list; each sizes to its content.
            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = 0f;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var selector = new SelectorBuilder().ViewModel(_viewModel).Build();
            selector.transform.SetParent(go.transform, false);

            var zoneList = new ZoneListBuilder().ViewModel(_viewModel).Build();
            zoneList.transform.SetParent(go.transform, false);

            return controller;
        }
    }
}
