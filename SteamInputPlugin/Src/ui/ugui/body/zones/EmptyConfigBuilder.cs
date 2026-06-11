using UnityEngine;
using UnityEngine.UI;
using TMPro;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.styles;
using com.github.lhervier.ksp.shared.ugui.sprites;
using com.github.lhervier.ksp.shared;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.zones
{
    /// <summary>
    /// Placeholder shown in place of the zones list when no controller config is selected.
    /// Mirrors the mockup's .kempty: uppercase title above two help paragraphs, with the
    /// "export" word highlighted in the accent color.
    /// </summary>
    public class EmptyConfigBuilder : IUGUIBuilder<EmptyConfigBuilder.EmptyConfigController>
    {
        public EmptyConfigController Build()
        {
            var go = new GameObject("EmptyConfig", typeof(RectTransform));
            
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

            return go.AddComponent<EmptyConfigController>();;
        }

        // .kempty-title: bold uppercase title above the body text.
        private static void BuildTitle(Transform parent)
        {
            var go = new GameObject("Title", typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var label = UGUILabels.AddLabel(go);
            label.text = ModLocalization.GetString("SteamInput_configHelpTitle").ToUpperInvariant();
            label.fontSize = SteamInputPalette.EmptyTitleFontSize;
            label.fontStyle = FontStyles.Bold;
            label.color = SteamInputPalette.EmptyTitleColor;
            label.alignment = TextAlignmentOptions.Left;
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

            var label = UGUILabels.AddLabel(go);
            label.text = intro + "\n\n" + refresh;
            label.richText = true;
            label.fontSize = SteamInputPalette.EmptyBodyFontSize;
            label.color = SteamInputPalette.EmptyBodyColor;
            label.alignment = TextAlignmentOptions.TopLeft;
            label.enableWordWrapping = true;

            // Wrapped text needs the fitter to claim its multi-line height.
            var fitter = go.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        public class EmptyConfigController : MonoBehaviour
        {
        }
    }
}
