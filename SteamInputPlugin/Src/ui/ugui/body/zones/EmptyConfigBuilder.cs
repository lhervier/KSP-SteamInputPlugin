using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.steaminput.ui.styles;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.zones
{
    /// <summary>
    /// Placeholder shown in place of the zones list when no controller config is selected.
    /// Mirrors the mockup's .kempty: uppercase title above two help paragraphs, with the
    /// "export" word highlighted in the accent color.
    /// </summary>
    public class EmptyConfigBuilder
    {
        private CheatSheetViewModel _viewModel;

        public EmptyConfigBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public EmptyConfigController Create()
        {
            var go = new GameObject("EmptyConfig", typeof(RectTransform));
            var controller = go.AddComponent<EmptyConfigController>();
            controller.BindViewModel(_viewModel);

            // .kempty: padded VLG stacking the title and the body paragraphs.
            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(
                SteamInputPalette.EmptyPaddingH,
                SteamInputPalette.EmptyPaddingH,
                SteamInputPalette.EmptyPaddingV,
                SteamInputPalette.EmptyPaddingV);
            layout.spacing = SteamInputPalette.EmptyTitleSpacing;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            BuildTitle(go.transform);
            BuildBody(go.transform);

            return controller;
        }

        // .kempty-title: bold uppercase title above the body text.
        private static void BuildTitle(Transform parent)
        {
            var go = new GameObject("Title", typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var label = go.AddComponent<Text>();
            label.text = ModLocalization.GetString("SteamInput_configHelpTitle").ToUpperInvariant();
            label.font = HighLogic.UISkin.font;
            label.fontSize = SteamInputPalette.EmptyTitleFontSize;
            label.fontStyle = FontStyle.Bold;
            label.color = SteamInputPalette.EmptyTitleColor;
            label.alignment = TextAnchor.MiddleLeft;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;
        }

        // .kempty body: two wrapped help paragraphs, separated by a blank line. The "export"
        // word is interpolated into the second paragraph wrapped in a rich-text color tag.
        private static void BuildBody(Transform parent)
        {
            var go = new GameObject("Body", typeof(RectTransform));
            go.transform.SetParent(parent, false);

            string accentHex = ColorUtility.ToHtmlStringRGB(SteamInputPalette.EmptyHighlightColor);
            string highlighted = "<color=#" + accentHex + ">" + ModLocalization.GetString("SteamInput_configHelpExport") + "</color>";
            string intro = ModLocalization.GetString("SteamInput_configHelpIntro");
            string refresh = ModLocalization.GetString("SteamInput_configHelpRefresh", highlighted);

            var label = go.AddComponent<Text>();
            label.text = intro + "\n\n" + refresh;
            label.supportRichText = true;
            label.font = HighLogic.UISkin.font;
            label.fontSize = SteamInputPalette.EmptyBodyFontSize;
            label.color = SteamInputPalette.EmptyBodyColor;
            label.alignment = TextAnchor.UpperLeft;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;

            // Wrapped text needs the fitter to claim its multi-line height.
            var fitter = go.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        public class EmptyConfigController : BaseSteamInputController
        {
        }
    }
}
