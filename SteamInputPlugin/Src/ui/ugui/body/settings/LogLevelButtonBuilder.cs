using UnityEngine;
using UnityEngine.UI;
using TMPro;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui.styles;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.sprites;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.settings
{
    /// <summary>
    /// The log level control. A "rotating" button (replaces the mockup's combobox): its label is the
    /// current log level; clicking it advances to the next level, wrapping back to the first one.
    /// The label is driven by the ViewModel's OnLogLevelChanged event, so it stays the single source
    /// of truth — the click only asks the ViewModel to switch level.
    /// </summary>
    public class LogLevelButtonBuilder : IUGUIBuilder<LogLevelButtonController>
    {
        // Rotation glyph hinting that the button cycles, with degraded alternatives picked at
        // build time depending on what the font (base + fallbacks) can actually render.
        private static string CycleGlyph => DefaultPalette.PickGlyph("↻", "⟳", "↺", "↓↑");

        private CheatSheetViewModel _viewModel;
        public LogLevelButtonBuilder ViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        public LogLevelButtonController Build()
        {
            var go = new GameObject("LogLevelButton", typeof(RectTransform));
            LogLevelButtonController controller = go
                .AddComponent<LogLevelButtonController>()
                .ViewModel(_viewModel);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.minHeight = SteamInputPalette.SettingsLogLevelHeight;
            layoutElement.preferredHeight = SteamInputPalette.SettingsLogLevelHeight;

            // Field fill + 1px border (same look as the config combo), white-tinted so the
            // Button's color block drives the hover feedback on top of the baked colors.
            var image = go.AddComponent<Image>();
            image.sprite = SpritesZones.ComboSprite;
            image.type = Image.Type.Sliced;
            image.color = Color.white;
            image.raycastTarget = true;

            var button = go.AddComponent<Button>();
            button.targetGraphic = image;
            var colors = button.colors;
            colors.normalColor = SteamInputPalette.SettingsLogLevelBgColor;
            colors.highlightedColor = SteamInputPalette.SettingsLogLevelHoverColor;
            colors.pressedColor = SteamInputPalette.SettingsLogLevelHoverColor;
            colors.selectedColor = SteamInputPalette.SettingsLogLevelBgColor;
            colors.disabledColor = SteamInputPalette.SettingsLogLevelBgColor;
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.1f;
            button.colors = colors;
            button.onClick.AddListener(() => controller.CycleLogLevel());

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(
                Mathf.RoundToInt(SteamInputPalette.SettingsLogLevelPaddingH),
                Mathf.RoundToInt(SteamInputPalette.SettingsLogLevelPaddingH),
                0, 0);
            layout.spacing = DefaultPalette.Spacing;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            // Current level label, greedy so the cycle glyph is pushed to the right edge.
            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(go.transform, false);
            var labelElement = labelGo.AddComponent<LayoutElement>();
            labelElement.flexibleWidth = 1f;

            var label = UGUILabels.AddLabel(labelGo);
            label.fontSize = SteamInputPalette.SettingsLogLevelFontSize;
            label.color = SteamInputPalette.SettingsLogLevelTextColor;
            label.alignment = TextAlignmentOptions.Left;

            // Rotate hint showing the button cycles through the levels. Sprite tag (no
            // circular-arrow glyph in the game fonts), text glyph as a fallback if the
            // texture is missing.
            var cycleGo = new GameObject("Cycle", typeof(RectTransform));
            cycleGo.transform.SetParent(go.transform, false);
            var cycle = UGUILabels.AddLabel(cycleGo);
            cycle.text = SpritesIcons.HasSprite("refresh") ? "<sprite name=\"refresh\" tint=1>" : CycleGlyph;
            cycle.fontSize = SteamInputPalette.SettingsLogLevelCycleFontSize;
            cycle.color = SteamInputPalette.SettingsLogLevelCycleColor;
            cycle.alignment = TextAlignmentOptions.Center;

            controller.Label(label);

            return controller;
        }
    }
}
