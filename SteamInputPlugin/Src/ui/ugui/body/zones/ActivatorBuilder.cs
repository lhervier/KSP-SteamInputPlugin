using UnityEngine;
using UnityEngine.UI;
using TMPro;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using com.github.lhervier.ksp.steaminput.ui.model;
using com.github.lhervier.ksp.shared.ugui;
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
        public ActivatorBuilder ViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        private UIActivator _activator;
        public ActivatorBuilder Activator(UIActivator activator)
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

            BuildChip(
                go.transform,
                "Kbd",
                SpritesActivators.ActivatorInputSprite,
                activator.IconText,
                SteamInputPalette.ActivatorInputTextColor,
                SteamInputPalette.ActivatorInputFontSize);

            if (!string.IsNullOrEmpty(activator.PressText))
            {
                BuildChip(
                    go.transform,
                    "Press",
                    SpritesActivators.ActivatorPressSprite,
                    activator.PressText,
                    SteamInputPalette.ActivatorPressTextColor,
                    SteamInputPalette.ActivatorPressFontSize);
            }
        }

        private void BuildChip(Transform parent, string name, Sprite sprite, string text, Color textColor, int fontSize)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var image = go.AddComponent<Image>();
            image.sprite = sprite;
            image.type = Image.Type.Sliced;
            image.color = Color.white;
            image.raycastTarget = false;

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(
                Mathf.RoundToInt(SteamInputPalette.ActivatorInputPaddingH),
                Mathf.RoundToInt(SteamInputPalette.ActivatorInputPaddingH),
                0, 0);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            var labelGo = new GameObject("Text", typeof(RectTransform));
            labelGo.transform.SetParent(go.transform, false);

            var label = labelGo.AddComponent<TextMeshProUGUI>();
            label.text = text;
            label.font = DefaultPalette.Font;
            label.fontSize = fontSize;
            label.color = textColor;
            label.alignment = TextAlignmentOptions.Center;
            label.enableWordWrapping = false;
            label.overflowMode = TextOverflowModes.Overflow;
            label.raycastTarget = false;
        }

        // .ksep : the "—" between the key and the action.
        private void BuildSeparator(Transform parent)
        {
            var go = new GameObject("Separator", typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var sep = go.AddComponent<TextMeshProUGUI>();
            sep.text = "—";
            sep.font = DefaultPalette.Font;
            sep.fontSize = SteamInputPalette.ActivatorSeparatorFontSize;
            sep.color = SteamInputPalette.ActivatorSeparatorColor;
            sep.alignment = TextAlignmentOptions.Center;
            sep.enableWordWrapping = false;
            sep.overflowMode = TextOverflowModes.Overflow;
            sep.raycastTarget = false;
        }

        private void BuildAction(Transform parent, UIActivator activator)
        {
            var go = new GameObject("Action", typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.flexibleWidth = 1f;

            var action = go.AddComponent<TextMeshProUGUI>();
            action.text = BuildActionText(activator);
            action.richText = true;
            action.font = DefaultPalette.Font;
            action.fontSize = SteamInputPalette.ActivatorActionFontSize;
            action.color = activator.Highlighted
                ? SteamInputPalette.ActivatorActionHighlightColor
                : SteamInputPalette.ActivatorActionColor;
            action.alignment = TextAlignmentOptions.Left;
            action.enableWordWrapping = true;
            action.overflowMode = TextOverflowModes.Overflow;
            action.raycastTarget = false;
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
