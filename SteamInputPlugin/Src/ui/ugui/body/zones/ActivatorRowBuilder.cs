using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using com.github.lhervier.ksp.ui.model;

namespace com.github.lhervier.ksp.ui.ugui.body.zones
{
    /// <summary>
    /// Builds one cheat-sheet row (mockup .krow):
    ///   [ .kkbd icon ] [ .kpress short/long ]   —   action label  (note)
    /// The data is already display-ready in the <see cref="UIActivator"/>.
    /// </summary>
    public class ActivatorRowBuilder
    {
        private CheatSheetViewModel _viewModel;

        public ActivatorRowBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public ActivatorRowController Create(UIActivator activator)
        {
            var go = new GameObject("ActivatorRow", typeof(RectTransform));
            ActivatorRowController controller = go.AddComponent<ActivatorRowController>();
            controller.Initialize(_viewModel);

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(
                0, 0,
                Mathf.RoundToInt(SteamInputPalette.RowVerticalPadding),
                Mathf.RoundToInt(SteamInputPalette.RowVerticalPadding));
            layout.spacing = SteamInputPalette.RowSeparatorPaddingH;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            BuildKeyColumn(go.transform, activator);
            BuildSeparator(go.transform);
            BuildAction(go.transform, activator);

            return controller;
        }

        // .kkey : fixed-width left column holding the key chip and the optional press chip.
        private void BuildKeyColumn(Transform parent, UIActivator activator)
        {
            var go = new GameObject("Key", typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = SteamInputPalette.RowKeySpacing;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.minWidth = SteamInputPalette.RowKeyMinWidth;
            layoutElement.flexibleWidth = 0f;

            BuildChip(
                go.transform,
                "Kbd",
                SpritesPhysicalZone.KeyChipSprite,
                activator.IconText,
                SteamInputPalette.RowKeyTextColor,
                SteamInputPalette.RowKeyFontSize);

            if (!string.IsNullOrEmpty(activator.PressText))
            {
                BuildChip(
                    go.transform,
                    "Press",
                    SpritesPhysicalZone.PressChipSprite,
                    activator.PressText,
                    SteamInputPalette.RowPressTextColor,
                    SteamInputPalette.RowPressFontSize);
            }
        }

        // .kkbd / .kpress : a bordered chip sized to its text.
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
                Mathf.RoundToInt(SteamInputPalette.RowChipPaddingH),
                Mathf.RoundToInt(SteamInputPalette.RowChipPaddingH),
                0, 0);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            var labelGo = new GameObject("Text", typeof(RectTransform));
            labelGo.transform.SetParent(go.transform, false);

            var label = labelGo.AddComponent<Text>();
            label.text = text;
            label.font = HighLogic.UISkin.font;
            label.fontSize = fontSize;
            label.color = textColor;
            label.alignment = TextAnchor.MiddleCenter;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;
        }

        // .ksep : the "—" between the key and the action.
        private void BuildSeparator(Transform parent)
        {
            var go = new GameObject("Sep", typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var sep = go.AddComponent<Text>();
            sep.text = "—";
            sep.font = HighLogic.UISkin.font;
            sep.fontSize = SteamInputPalette.RowSeparatorFontSize;
            sep.color = SteamInputPalette.RowSeparatorColor;
            sep.alignment = TextAnchor.MiddleCenter;
            sep.horizontalOverflow = HorizontalWrapMode.Overflow;
            sep.verticalOverflow = VerticalWrapMode.Overflow;
            sep.raycastTarget = false;
        }

        // .kaction (+ .knote) : the action label, highlighted with a trailing note for a mode shift.
        private void BuildAction(Transform parent, UIActivator activator)
        {
            var go = new GameObject("Action", typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.flexibleWidth = 1f;

            var action = go.AddComponent<Text>();
            action.text = BuildActionText(activator);
            action.supportRichText = true;
            action.font = HighLogic.UISkin.font;
            action.fontSize = SteamInputPalette.RowActionFontSize;
            action.color = activator.Highlighted
                ? SteamInputPalette.RowActionHighlightColor
                : SteamInputPalette.RowActionColor;
            action.alignment = TextAnchor.MiddleLeft;
            action.horizontalOverflow = HorizontalWrapMode.Wrap;
            action.verticalOverflow = VerticalWrapMode.Overflow;
            action.raycastTarget = false;
        }

        private static string BuildActionText(UIActivator activator)
        {
            string text = activator.ActionText ?? "";
            if (string.IsNullOrEmpty(activator.Note))
            {
                return text;
            }
            string noteHex = ColorUtility.ToHtmlStringRGB(SteamInputPalette.RowNoteColor);
            return text + " <color=#" + noteHex + ">" + activator.Note + "</color>";
        }

        public class ActivatorRowController : BaseSteamInputController
        {
        }
    }
}
