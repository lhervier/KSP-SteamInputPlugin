using UnityEngine;
using UnityEngine.UI;
using TMPro;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.model;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.badge;
using com.github.lhervier.ksp.shared.ugui.styles;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.zones
{
    /// <summary>
    /// Builds one cheat-sheet row (mockup .krow):
    ///   [ .kkbd icon ] [ .kpress short/long ]   —   action label  (note)
    /// The data is already display-ready in the <see cref="UIActivator"/>.
    /// </summary>
    public class ActivatorBuilder : IUGUIBuilder<ActivatorBuilder.ActivatorController>
    {
        // ================================================
        // Builder parameters
        // ================================================

        private CheatSheetViewModel _viewModel;
        public ActivatorBuilder WithViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        private UIActivator _activator;
        public ActivatorBuilder WithActivator(UIActivator activator)
        {
            this._activator = activator;
            return this;
        }

        // =========================================
        // Build
        // =========================================

        public ActivatorController Build()
        {
            var go = new GameObject("Activator", typeof(RectTransform));

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(
                0, 0,
                Mathf.RoundToInt(SteamInputPalette.ActivatorPaddingV),
                Mathf.RoundToInt(SteamInputPalette.ActivatorPaddingV));
            layout.spacing = SteamInputPalette.ActivatorSeparatorPaddingH;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            BuildInputColumn(go.transform, _activator);
            BuildSeparator(go.transform);
            BuildAction(go.transform, _activator);

            return go.AddComponent<ActivatorController>();
        }

        // .kkey : fixed-width left column holding the key chip and the optional press chip.
        private void BuildInputColumn(Transform parent, UIActivator activator)
        {
            var go = new GameObject("Input", typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = SteamInputPalette.ActivatorInputSpacing;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.minWidth = SteamInputPalette.ActivatorInputMinWidth;
            layoutElement.flexibleWidth = 0f;

            // .kkbd : keyboard/button key chip. Square floor so short keys stay square.
            new BadgeBuilder()
                .WithParent(go.transform)
                .WithObjectName("Kbd")
                .WithText(activator.IconText)
                .WithColors(
                    SteamInputPalette.ActivatorInputTextColor,
                    SteamInputPalette.ActivatorInputBgColor,
                    SteamInputPalette.ActivatorInputBorderColor)
                .WithBorderThickness(SteamInputPalette.ActivatorInputBorderThickness)
                .WithFontSize(SteamInputPalette.ActivatorInputFontSize)
                .WithPadding(SteamInputPalette.ActivatorInputPaddingH, 0)
                .WithMinSize(SteamInputPalette.ActivatorInputChipMinSize)
                .Build();

            // .kpress : optional long-press chip, hugging its content (no square floor).
            if (!string.IsNullOrEmpty(activator.PressText))
            {
                new BadgeBuilder()
                    .WithParent(go.transform)
                    .WithObjectName("Press")
                    .WithText(activator.PressText)
                    .WithColors(
                        SteamInputPalette.ActivatorPressTextColor,
                        SteamInputPalette.ActivatorPressBgColor,
                        SteamInputPalette.ActivatorPressBorderColor)
                    .WithBorderThickness(SteamInputPalette.ActivatorPressBorderThickness)
                    .WithFontSize(SteamInputPalette.ActivatorPressFontSize)
                    .WithPadding(SteamInputPalette.ActivatorInputPaddingH, 0)
                    .Build();
            }
        }

        // .ksep : the "—" between the key and the action.
        private void BuildSeparator(Transform parent)
        {
            var go = new GameObject("Separator", typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var sep = UGUILabels.AddLabel(go);
            sep.text = "—";
            sep.fontSize = SteamInputPalette.ActivatorSeparatorFontSize;
            sep.color = SteamInputPalette.ActivatorSeparatorColor;
            sep.alignment = TextAlignmentOptions.Center;
        }

        private void BuildAction(Transform parent, UIActivator activator)
        {
            var go = new GameObject("Action", typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.flexibleWidth = 1f;

            var action = UGUILabels.AddLabel(go);
            action.text = BuildActionText(activator);
            action.richText = true;
            action.fontSize = SteamInputPalette.ActivatorActionFontSize;
            action.color = activator.Highlighted
                ? SteamInputPalette.ActivatorActionHighlightColor
                : SteamInputPalette.ActivatorActionColor;
            action.alignment = TextAlignmentOptions.Left;
            action.enableWordWrapping = true;
        }

        private static string BuildActionText(UIActivator activator)
        {
            string text = activator.ActionText ?? "";
            if (string.IsNullOrEmpty(activator.Note))
            {
                return text;
            }
            string noteHex = ColorUtility.ToHtmlStringRGB(SteamInputPalette.ActivatorNoteColor);
            return text + " <color=#" + noteHex + ">" + activator.Note + "</color>";
        }

        public class ActivatorController : MonoBehaviour
        {
        }
    }
}
