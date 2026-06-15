using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.styles;
using com.github.lhervier.ksp.shared.ugui.button;
using System.Data;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.titleBar
{
    public class TitleBarBuilder : IUGUIBuilder<TitleBarController>
    {
        // ==============================================
        // Builder parameters
        // ==============================================
        private CheatSheetViewModel _viewModel;
        public TitleBarBuilder WithViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        // ======================================
        // Build
        // ======================================

        public TitleBarController Build()
        {
            var rightRowGo = new GameObject("SteamInput.TitleBar.RightColumn", typeof(RectTransform));
            
            // Horizontal layout containing the right-side placeholders, sized to their text content
            var rightRowLayout = rightRowGo.AddComponent<HorizontalLayoutGroup>();
            rightRowLayout.spacing = DefaultPalette.Spacing;
            rightRowLayout.childAlignment = TextAnchor.MiddleLeft;
            rightRowLayout.childControlWidth = true;
            rightRowLayout.childControlHeight = true;
            rightRowLayout.childForceExpandWidth = false;
            rightRowLayout.childForceExpandHeight = false;

            var actionGroupLabelController = new ActionGroupLabelBuilder().WithViewModel(_viewModel).Build();
            actionGroupLabelController.transform.SetParent(rightRowGo.transform, false);

            var controllerGo = new GamepadLabelBuilder().WithViewModel(_viewModel).Build();
            controllerGo.transform.SetParent(rightRowGo.transform, false);

            // "…" (U+2026) instead of "⋯" (U+22EF): the game's TMP font atlas does not contain the
            // math midline ellipsis, while U+2026 is basic punctuation (also used by TMP's own
            // Ellipsis overflow mode).
            var menuButtonController = new ButtonBuilder()
                .WithObjectName("SteamInput.TitleBar.MenuButton")
                .WithLabel("…")
                .WithInteractableState(true)
                .WithBackgroundColor(PopupPalette.TitleBarButtonColor)
                .WithHoverColor(PopupPalette.TitleBarButtonHoverColor)
                .Build();
            menuButtonController.transform.SetParent(rightRowGo.transform, false);
            
            return rightRowGo
                .AddComponent<TitleBarController>()
                .WithViewModel(_viewModel)
                .WithMenuButtonController(menuButtonController);
        }
    }
}