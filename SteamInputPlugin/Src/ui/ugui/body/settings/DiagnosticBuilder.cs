using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.ugui.shared.styles;
using com.github.lhervier.ksp.ugui.shared.sprites;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.settings
{
    /// <summary>
    /// "Diagnostic" section, mockup .kset-note / .kset-kv / .kset-ctx. Read-only info aimed at the
    /// mod maintainers: an amber note, the controller-connected badge, and the list of activated
    /// contexts. Mirrors what the legacy IMGUI window showed (controller connected + contexts).
    /// </summary>
    public class DiagnosticBuilder
    {
        private const string CtxDaemonSuffix = "CtxDaemon";

        private static Sprite _noteSprite;
        private static Sprite _contextSprite;
        private static Sprite _badgeOkSprite;
        private static Sprite _badgeNoSprite;

        private CheatSheetViewModel _viewModel;

        public DiagnosticBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public DiagnosticController Create()
        {
            var go = new GameObject("Diagnostic", typeof(RectTransform));
            DiagnosticController controller = go.AddComponent<DiagnosticController>();
            controller.Initialize(_viewModel);

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
            var label = go.AddComponent<Text>();
            label.text = (text ?? "").ToUpperInvariant();
            label.font = HighLogic.UISkin.font;
            label.fontSize = SteamInputPalette.SettingsLabelFontSize;
            label.fontStyle = FontStyle.Bold;
            label.color = SteamInputPalette.SettingsLabelColor;
            label.alignment = TextAnchor.MiddleLeft;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;
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
            var label = textGo.AddComponent<Text>();
            label.text = text;
            label.font = HighLogic.UISkin.font;
            label.fontSize = SteamInputPalette.SettingsHintFontSize;
            label.color = SteamInputPalette.SettingsNoteTextColor;
            label.alignment = TextAnchor.UpperLeft;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;

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
            var key = keyGo.AddComponent<Text>();
            key.text = ModLocalization.GetString("SteamInput_settings_connected");
            key.font = HighLogic.UISkin.font;
            key.fontSize = SteamInputPalette.SettingsKvFontSize;
            key.color = SteamInputPalette.SettingsKvKeyColor;
            key.alignment = TextAnchor.MiddleLeft;
            key.horizontalOverflow = HorizontalWrapMode.Overflow;
            key.verticalOverflow = VerticalWrapMode.Overflow;
            key.raycastTarget = false;

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
            var badgeText = badgeTextGo.AddComponent<Text>();
            badgeText.font = HighLogic.UISkin.font;
            badgeText.fontSize = SteamInputPalette.SettingsBadgeFontSize;
            badgeText.fontStyle = FontStyle.Bold;
            badgeText.alignment = TextAnchor.MiddleCenter;
            badgeText.horizontalOverflow = HorizontalWrapMode.Overflow;
            badgeText.verticalOverflow = VerticalWrapMode.Overflow;
            badgeText.raycastTarget = false;

            controller.BindBadge(badgeImage, badgeText, _badgeOkSprite, _badgeNoSprite);
        }

        // ".kset-sub": small uppercase grey sub-header above the contexts box.
        private static void BuildSubLabel(Transform parent, string text)
        {
            var go = new GameObject("SubLabel", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var label = go.AddComponent<Text>();
            label.text = (text ?? "").ToUpperInvariant();
            label.font = HighLogic.UISkin.font;
            label.fontSize = SteamInputPalette.SettingsSubFontSize;
            label.fontStyle = FontStyle.Bold;
            label.color = SteamInputPalette.SettingsSubColor;
            label.alignment = TextAnchor.MiddleLeft;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;
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

            controller.BindContextsBox(go.transform);
        }

        public class DiagnosticController : BaseSteamInputController
        {
            private Image _badgeImage;
            private Text _badgeText;
            private Sprite _badgeOkSprite;
            private Sprite _badgeNoSprite;

            private Transform _contextsBox;
            private readonly List<GameObject> _contextRows = new List<GameObject>();

            public void BindBadge(Image image, Text text, Sprite okSprite, Sprite noSprite)
            {
                this._badgeImage = image;
                this._badgeText = text;
                this._badgeOkSprite = okSprite;
                this._badgeNoSprite = noSprite;
            }

            public void BindContextsBox(Transform contextsBox)
            {
                this._contextsBox = contextsBox;
            }

            public void Start()
            {
                if (ViewModel == null)
                {
                    return;
                }
                ViewModel.OnGamepadConnected.Add(OnGamepadConnected);
                ViewModel.OnActivatedContextsChanged.Add(OnActivatedContextsChanged);
                OnGamepadConnected(ViewModel.GamepadConnected);
                OnActivatedContextsChanged(ViewModel.ActivatedContexts);
            }

            public void OnDestroy()
            {
                if (ViewModel == null)
                {
                    return;
                }
                ViewModel.OnGamepadConnected.Remove(OnGamepadConnected);
                ViewModel.OnActivatedContextsChanged.Remove(OnActivatedContextsChanged);
            }

            private void OnGamepadConnected(bool connected)
            {
                if (_badgeImage == null || _badgeText == null)
                {
                    return;
                }
                _badgeImage.sprite = connected ? _badgeOkSprite : _badgeNoSprite;
                _badgeText.text = ModLocalization.GetString(connected ? "SteamInput_yes" : "SteamInput_no").ToUpperInvariant();
                _badgeText.color = connected
                    ? SteamInputPalette.SettingsBadgeOkTextColor
                    : SteamInputPalette.SettingsBadgeNoTextColor;
            }

            private void OnActivatedContextsChanged(List<string> contexts)
            {
                if (_contextsBox == null)
                {
                    return;
                }

                foreach (GameObject row in _contextRows)
                {
                    Destroy(row);
                }
                _contextRows.Clear();

                if (contexts == null || contexts.Count == 0)
                {
                    _contextRows.Add(BuildEmptyRow());
                    return;
                }
                foreach (string context in contexts)
                {
                    _contextRows.Add(BuildContextRow(StripDaemonSuffix(context)));
                }
            }

            private GameObject BuildContextRow(string text)
            {
                var go = new GameObject("ContextRow", typeof(RectTransform));
                go.transform.SetParent(_contextsBox, false);
                var label = go.AddComponent<Text>();
                label.text = "› " + text;
                label.font = HighLogic.UISkin.font;
                label.fontSize = SteamInputPalette.SettingsContextFontSize;
                label.color = SteamInputPalette.SettingsContextRowColor;
                label.alignment = TextAnchor.MiddleLeft;
                label.horizontalOverflow = HorizontalWrapMode.Overflow;
                label.verticalOverflow = VerticalWrapMode.Overflow;
                label.raycastTarget = false;
                ApplyRowPadding(label);
                return go;
            }

            private GameObject BuildEmptyRow()
            {
                var go = new GameObject("ContextEmpty", typeof(RectTransform));
                go.transform.SetParent(_contextsBox, false);
                var label = go.AddComponent<Text>();
                label.text = ModLocalization.GetString("SteamInput_settings_noContext");
                label.font = HighLogic.UISkin.font;
                label.fontSize = SteamInputPalette.SettingsContextFontSize;
                label.fontStyle = FontStyle.Italic;
                label.color = SteamInputPalette.SettingsContextEmptyColor;
                label.alignment = TextAnchor.MiddleLeft;
                label.horizontalOverflow = HorizontalWrapMode.Overflow;
                label.verticalOverflow = VerticalWrapMode.Overflow;
                label.raycastTarget = false;
                ApplyRowPadding(label);
                return go;
            }

            // Horizontal inset is carried by the box's VLG padding; each row only fixes its height
            // so successive rows keep a small, even gap (mockup .kset-ctx-row vertical padding).
            private static void ApplyRowPadding(Text label)
            {
                var element = label.gameObject.AddComponent<LayoutElement>();
                element.minHeight = SteamInputPalette.SettingsContextFontSize + 6f;
            }

            private static string StripDaemonSuffix(string context)
            {
                if (!string.IsNullOrEmpty(context) && context.EndsWith(CtxDaemonSuffix))
                {
                    return context.Substring(0, context.Length - CtxDaemonSuffix.Length);
                }
                return context;
            }
        }
    }
}
