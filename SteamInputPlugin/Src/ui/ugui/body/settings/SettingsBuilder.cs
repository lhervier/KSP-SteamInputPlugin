using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.shared.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui.button;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.settings
{
    /// <summary>
    /// The settings screen shown in place of the cheat sheet (see BodyBuilder.OnShowSettings).
    /// Mirrors the mockup's #view-settings: a head with a back button, a "Logging" section (the
    /// log level rotating button + a hint), and a "Diagnostic" section. Stacked top to bottom.
    /// </summary>
    public class SettingsBuilder
    {
        private const string BackGlyph = "‹"; // ‹ (U+2039)

        private CheatSheetViewModel _viewModel;
        private ButtonBuilder _buttonBuilder;
        private LogLevelButtonBuilder _logLevelButtonBuilder;
        private DiagnosticBuilder _diagnosticBuilder;

        private static Sprite _hintSprite;

        public SettingsBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._buttonBuilder = new ButtonBuilder();
            this._logLevelButtonBuilder = new LogLevelButtonBuilder(viewModel);
            this._diagnosticBuilder = new DiagnosticBuilder(viewModel);
        }

        public SettingsController Create()
        {
            var go = new GameObject("SteamInput.Body.Settings", typeof(RectTransform));
            var controller = go.AddComponent<SettingsController>();
            controller.Initialize(_viewModel);

            // Stacks the head and the sections back-to-back; each section sizes to its content.
            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = 0f;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            BuildHead(controller, go.transform);
            BuildSeparator(go.transform);

            BuildLoggingSection(go.transform);
            BuildSeparator(go.transform);

            DiagnosticBuilder.DiagnosticController diagnostic = _diagnosticBuilder.Create();
            diagnostic.transform.SetParent(go.transform, false);

            return controller;
        }

        // Head: back button (returns to the cheat sheet) + the "Settings" title. Mockup .kset-head.
        private void BuildHead(SettingsController controller, Transform parent)
        {
            var go = new GameObject("Head", typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var image = go.AddComponent<Image>();
            image.sprite = SpritesGlobal.FillSprite;
            image.type = Image.Type.Simple;
            image.color = SteamInputPalette.SettingsHeadBgColor;
            image.raycastTarget = false;

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(
                SteamInputPalette.SettingsHeadPaddingH,
                SteamInputPalette.SettingsHeadPaddingH,
                SteamInputPalette.SettingsHeadPaddingV,
                SteamInputPalette.SettingsHeadPaddingV);
            layout.spacing = SteamInputPalette.SettingsHeadSpacing;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            _buttonBuilder.SetObjectName("Back");
            _buttonBuilder.SetLabel(BackGlyph);
            ButtonController back = _buttonBuilder.Build();
            back.transform.SetParent(go.transform, false);
            controller.BindBackButtonController(back);

            var titleGo = new GameObject("Title", typeof(RectTransform));
            titleGo.transform.SetParent(go.transform, false);
            var titleElement = titleGo.AddComponent<LayoutElement>();
            titleElement.flexibleWidth = 1f;

            var title = titleGo.AddComponent<Text>();
            title.text = ModLocalization.GetString("SteamInput_settings").ToUpperInvariant();
            title.font = HighLogic.UISkin.font;
            title.fontSize = SteamInputPalette.SettingsHeadTitleFontSize;
            title.fontStyle = FontStyle.Bold;
            title.color = SteamInputPalette.SettingsHeadTitleColor;
            title.alignment = TextAnchor.MiddleLeft;
            title.horizontalOverflow = HorizontalWrapMode.Overflow;
            title.verticalOverflow = VerticalWrapMode.Overflow;
            title.raycastTarget = false;
        }

        // Logging section: section label + the log level rotating button + a hint box.
        private void BuildLoggingSection(Transform parent)
        {
            Transform section = BuildSection(parent, "LoggingSection");

            BuildSectionLabel(section, ModLocalization.GetString("SteamInput_settings_logging"));

            LogLevelButtonBuilder.LogLevelButtonController logLevel = _logLevelButtonBuilder.Create();
            logLevel.transform.SetParent(section, false);

            BuildHint(section, ModLocalization.GetString("SteamInput_settings_loggingHint"));
        }

        // ".kset-section": padded vertical container for a section's content.
        private static Transform BuildSection(Transform parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);

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

            return go.transform;
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

        // ".kset-hint": boxed grey help text under a control.
        private static void BuildHint(Transform parent, string text)
        {
            if (_hintSprite == null)
            {
                _hintSprite = SpritesGlobal.MakeChipSprite(
                    SteamInputPalette.SettingsHintBgColor,
                    SteamInputPalette.SettingsHintBorderColor,
                    1);
            }

            var go = new GameObject("Hint", typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var image = go.AddComponent<Image>();
            image.sprite = _hintSprite;
            image.type = Image.Type.Sliced;
            image.color = Color.white;
            image.raycastTarget = false;

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(
                SteamInputPalette.SettingsHintPaddingH,
                SteamInputPalette.SettingsHintPaddingH,
                SteamInputPalette.SettingsHintPaddingV,
                SteamInputPalette.SettingsHintPaddingV);
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
            label.color = SteamInputPalette.SettingsHintTextColor;
            label.alignment = TextAnchor.UpperLeft;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;

            // Wrapped text needs the fitter to claim its multi-line height.
            var fitter = textGo.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        // ".kset-sep": 1px horizontal divider between the head and the sections.
        private static void BuildSeparator(Transform parent)
        {
            var go = new GameObject("Separator", typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.minHeight = 1f;
            layoutElement.preferredHeight = 1f;

            var image = go.AddComponent<Image>();
            image.sprite = SpritesGlobal.FillSprite;
            image.type = Image.Type.Simple;
            image.color = SteamInputPalette.SettingsSeparatorColor;
            image.raycastTarget = false;
        }

        public class SettingsController : BaseSteamInputController
        {
            private ButtonController _backButtonController;

            public void BindBackButtonController(ButtonController backButtonController)
            {
                this._backButtonController = backButtonController;
            }

            public void Start()
            {
                if( this._backButtonController != null )
                {
                    this._backButtonController.OnClick.Add(ViewModel.CloseSettings);
                }
            }

            public void OnDestroy()
            {
                if( this._backButtonController != null )
                {
                    this._backButtonController.OnClick.Remove(ViewModel.CloseSettings);
                }
            }
        }
    }
}
