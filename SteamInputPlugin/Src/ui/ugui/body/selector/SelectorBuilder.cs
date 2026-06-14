using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.styles;
using com.github.lhervier.ksp.shared.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui.button;
using com.github.lhervier.ksp.shared.ugui.combo;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.selector
{
    /// <summary>
    /// Config picker shown at the top of the body: a shared combobox listing the available configs
    /// (title + controller type, plus a "none" entry) and a refresh button to rescan the folder.
    /// The combo is driven by the ViewModel through SelectorController; this builder just assembles
    /// the row and wires the combo's rendering (LabelFor) and item content to the controller.
    /// </summary>
    public class SelectorBuilder : IUGUIBuilder<SelectorController>
    {
        // Preferred glyph with degraded alternatives: the game's TMP atlases do not necessarily
        // contain it; the actual glyph is picked at build time. Used only if the sprite is missing.
        private static string RefreshGlyph => DefaultPalette.PickGlyph("↻", "⟳", "↺", "↓↑");

        // ==================================
        // Builder parameters
        // ==================================

        private CheatSheetViewModel _viewModel;
        public SelectorBuilder ViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        // =================================
        // Build
        // =================================

        public SelectorController Build()
        {
            var go = new GameObject("SteamInput.Body.Selector", typeof(RectTransform));
            SelectorController controller = go.AddComponent<SelectorController>();
            controller.ViewModel(_viewModel);

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(
                Mathf.RoundToInt(DefaultPalette.PaddingLeft),
                Mathf.RoundToInt(DefaultPalette.PaddingRight),
                Mathf.RoundToInt(SteamInputPalette.ComboPaddingV),
                Mathf.RoundToInt(SteamInputPalette.ComboPaddingV));
            layout.spacing = DefaultPalette.Spacing;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            // The combobox: shared component. It renders config options (title + type) and reports the
            // selection back to the controller, which forwards it to the ViewModel.
            ComboController combo = new ComboBuilder()
                .Parent(go.transform)
                .LabelFor(controller.LabelFor)
                .ItemContent(new ConfigComboItemContentBuilder().Configs(controller.ResolveConfig))
                .Build();
            controller.Combo(combo);

            // Stretch the combo to fill the row; the refresh button keeps its fixed square size.
            var comboLe = combo.gameObject.AddComponent<LayoutElement>();
            comboLe.flexibleWidth = 1f;

            // Refresh button: triggers a rescan of the Steam config folder. Sized to match the combo
            // height so the icon reads at a glance. Sprite tag, with a text glyph fallback.
            ButtonController refresh = new ButtonBuilder()
                .ObjectName("Refresh")
                .Label(SpritesIcons.HasSprite("refresh") ? "<sprite name=\"refresh\" tint=1>" : RefreshGlyph)
                .Build();
            refresh.transform.SetParent(go.transform, false);
            controller.RefreshButton(refresh);

            var refreshLayout = refresh.GetComponent<LayoutElement>();
            refreshLayout.minWidth = SteamInputPalette.ComboHeight;
            refreshLayout.minHeight = SteamInputPalette.ComboHeight;
            refreshLayout.preferredWidth = SteamInputPalette.ComboHeight;
            refreshLayout.preferredHeight = SteamInputPalette.ComboHeight;

            return controller;
        }
    }
}
