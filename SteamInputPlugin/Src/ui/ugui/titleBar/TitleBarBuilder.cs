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
        public TitleBarBuilder ViewModel(CheatSheetViewModel viewModel)
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

            var actionGroupLabelController = new ActionGroupLabelBuilder().ViewModel(_viewModel).Build();
            actionGroupLabelController.transform.SetParent(rightRowGo.transform, false);

            var controllerGo = new GamepadLabelBuilder().ViewModel(_viewModel).Build();
            controllerGo.transform.SetParent(rightRowGo.transform, false);

            var menuButtonController = new ButtonBuilder()
                .ObjectName("SteamInput.TitleBar.MenuButton")
                .Label("⋯")
                .Interactable(true)
                .BackgroundColor(PopupPalette.TitleBarButtonColor)
                .HoverColor(PopupPalette.TitleBarButtonHoverColor)
                .Build();
            menuButtonController.transform.SetParent(rightRowGo.transform, false);
            
            return rightRowGo
                .AddComponent<TitleBarController>()
                .ViewModel(_viewModel)
                .MenuButtonController(menuButtonController);
        }
    }
}