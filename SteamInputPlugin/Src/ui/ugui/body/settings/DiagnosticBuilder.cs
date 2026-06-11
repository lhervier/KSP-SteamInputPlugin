using UnityEngine;
using UnityEngine.UI;
using TMPro;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.shared.ugui.styles;
using com.github.lhervier.ksp.shared.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.settings
{
    /// <summary>
    /// "Diagnostic" section, mockup .kset-note / .kset-kv / .kset-ctx. Read-only info aimed at the
    /// mod maintainers: an amber note, the controller-connected badge, and the list of activated
    /// contexts. Mirrors what the legacy IMGUI window showed (controller connected + contexts).
    /// </summary>
    public class DiagnosticBuilder : IUGUIBuilder<DiagnosticController>
    {
        private static Sprite _noteSprite;
        private static Sprite _contextSprite;
        private static Sprite _badgeOkSprite;
        private static Sprite _badgeNoSprite;

        // ====================================
        // Builder parameters
        // ====================================

        private CheatSheetViewModel _viewModel;
        public DiagnosticBuilder ViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        // =========================
        // Build
        // =========================

        public DiagnosticController Build()
        {
            var go = new GameObject("Diagnostic", typeof(RectTransform));
            DiagnosticController controller = go
                .AddComponent<DiagnosticController>()
                .ViewModel(_viewModel);

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(
                SteamInputPalette.SettingsSectionPaddingH,
                SteamInputPalette.SettingsSectionPaddingH,
                SteamInputPalette.SettingsSectionPaddingTop,
                SteamInputPalette.SettingsSectionPaddingBottom);
            layout.spacing = SteamInputPalette.SettingsSectionSpacing;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            BuildSectionLabel(go.transform, ModLocalization.GetString("SteamInput_settings_diagnostic"));
            BuildNote(go.transform, ModLocalization.GetString("SteamInput_settings_diagnosticNote"));
            BuildConnectedRow(go.transform, controller);
            BuildSubLabel(go.transform, ModLocalization.GetString("SteamInput_settings_contexts"));
            BuildContextsBox(go.transform, controller);

            return controller;
        }

        // ".kset-label": accent, uppercase section title.
        private static void BuildSectionLabel(Transform parent, string text)
        {
            var go = new GameObject("SectionLabel", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var label = UGUILabels.AddLabel(go);
            label.text = (text ?? "").ToUpperInvariant();
            label.fontSize = SteamInputPalette.SettingsLabelFontSize;
            label.fontStyle = FontStyles.Bold;
            label.color = SteamInputPalette.SettingsLabelColor;
            label.alignment = TextAlignmentOptions.Left;
        }

        // ".kset-note": amber boxed maintainer note.
        private static void BuildNote(Transform parent, string text)
        {
            if (_noteSprite == null)
            {
                _noteSprite = SpritesGlobal.MakeChipSprite(
                    SteamInputPalette.SettingsNoteBgColor,
                    SteamInputPalette.SettingsNoteBorderColor,
                    1);
            }

            var go = new GameObject("Note", typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var image = go.AddComponent<Image>();
            image.sprite = _noteSprite;
            image.type = Image.Type.Sliced;
            image.color = Color.white;
            image.raycastTarget = false;

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(
                SteamInputPalette.SettingsNotePaddingH,
                SteamInputPalette.SettingsNotePaddingH,
                SteamInputPalette.SettingsNotePaddingV,
                SteamInputPalette.SettingsNotePaddingV);
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var textGo = new GameObject("Text", typeof(RectTransform));
            textGo.transform.SetParent(go.transform, false);
            var label = UGUILabels.AddLabel(textGo);
            label.text = text;
            label.fontSize = SteamInputPalette.SettingsHintFontSize;
            label.color = SteamInputPalette.SettingsNoteTextColor;
            label.alignment = TextAlignmentOptions.TopLeft;
            label.enableWordWrapping = true;

            // Wrapped text needs the fitter to claim its multi-line height.
            var fitter = textGo.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        // ".kset-kv": "Controller connected" + an OK/NO badge.
        private void BuildConnectedRow(Transform parent, DiagnosticController controller)
        {
            var go = new GameObject("ConnectedRow", typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.minHeight = SteamInputPalette.SettingsKvHeight;
            layoutElement.preferredHeight = SteamInputPalette.SettingsKvHeight;

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = DefaultPalette.Spacing;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            var keyGo = new GameObject("Key", typeof(RectTransform));
            keyGo.transform.SetParent(go.transform, false);
            var keyElement = keyGo.AddComponent<LayoutElement>();
            keyElement.flexibleWidth = 1f;
            var key = UGUILabels.AddLabel(keyGo);
            key.text = ModLocalization.GetString("SteamInput_settings_connected");
            key.fontSize = SteamInputPalette.SettingsKvFontSize;
            key.color = SteamInputPalette.SettingsKvKeyColor;
            key.alignment = TextAlignmentOptions.Left;

            // Badge: a chip whose sprite + colors are swapped by the controller on connect/disconnect.
            if (_badgeOkSprite == null)
            {
                _badgeOkSprite = SpritesGlobal.MakeChipSprite(
                    SteamInputPalette.SettingsBadgeOkBgColor,
                    SteamInputPalette.SettingsBadgeOkBorderColor,
                    1);
            }
            if (_badgeNoSprite == null)
            {
                _badgeNoSprite = SpritesGlobal.MakeChipSprite(
                    SteamInputPalette.SettingsBadgeNoBgColor,
                    SteamInputPalette.SettingsBadgeNoBorderColor,
                    1);
            }

            var badgeGo = new GameObject("Badge", typeof(RectTransform));
            badgeGo.transform.SetParent(go.transform, false);
            var badgeImage = badgeGo.AddComponent<Image>();
            badgeImage.type = Image.Type.Sliced;
            badgeImage.color = Color.white;
            badgeImage.raycastTarget = false;

            var badgeLayout = badgeGo.AddComponent<HorizontalLayoutGroup>();
            badgeLayout.padding = new RectOffset(
                SteamInputPalette.SettingsBadgePaddingH,
                SteamInputPalette.SettingsBadgePaddingH,
                2, 2);
            badgeLayout.childAlignment = TextAnchor.MiddleCenter;
            badgeLayout.childControlWidth = true;
            badgeLayout.childControlHeight = true;
            badgeLayout.childForceExpandWidth = false;
            badgeLayout.childForceExpandHeight = false;

            // The chip hugs its label: the parent row HLG (childControlWidth, no force-expand) sizes
            // the badge to the preferred width reported by this inner HLG.
            var badgeTextGo = new GameObject("Text", typeof(RectTransform));
            badgeTextGo.transform.SetParent(badgeGo.transform, false);
            var badgeText = UGUILabels.AddLabel(badgeTextGo);
            badgeText.fontSize = SteamInputPalette.SettingsBadgeFontSize;
            badgeText.fontStyle = FontStyles.Bold;
            badgeText.alignment = TextAlignmentOptions.Center;

            controller.Badge(badgeImage, badgeText, _badgeOkSprite, _badgeNoSprite);
        }

        // ".kset-sub": small uppercase grey sub-header above the contexts box.
        private static void BuildSubLabel(Transform parent, string text)
        {
            var go = new GameObject("SubLabel", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var label = UGUILabels.AddLabel(go);
            label.text = (text ?? "").ToUpperInvariant();
            label.fontSize = SteamInputPalette.SettingsSubFontSize;
            label.fontStyle = FontStyles.Bold;
            label.color = SteamInputPalette.SettingsSubColor;
            label.alignment = TextAlignmentOptions.Left;
        }

        // ".kset-ctx": dark box that the controller fills with one row per activated context.
        private void BuildContextsBox(Transform parent, DiagnosticController controller)
        {
            if (_contextSprite == null)
            {
                _contextSprite = SpritesGlobal.MakeChipSprite(
                    SteamInputPalette.SettingsContextBgColor,
                    SteamInputPalette.SettingsContextBorderColor,
                    1);
            }

            var go = new GameObject("ContextsBox", typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var image = go.AddComponent<Image>();
            image.sprite = _contextSprite;
            image.type = Image.Type.Sliced;
            image.color = Color.white;
            image.raycastTarget = false;

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(
                SteamInputPalette.SettingsContextPaddingH,
                SteamInputPalette.SettingsContextPaddingH,
                SteamInputPalette.SettingsContextPaddingV,
                SteamInputPalette.SettingsContextPaddingV);
            layout.spacing = 0f;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var fitter = go.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            controller.ContextsBox(go.transform);
        }
    }
}
