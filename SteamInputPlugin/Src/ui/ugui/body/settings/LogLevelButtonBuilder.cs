using System;
using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui.styles;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.settings
{
    /// <summary>
    /// The log level control. A "rotating" button (replaces the mockup's combobox): its label is the
    /// current log level; clicking it advances to the next level, wrapping back to the first one.
    /// The label is driven by the ViewModel's OnLogLevelChanged event, so it stays the single source
    /// of truth — the click only asks the ViewModel to switch level.
    /// </summary>
    public class LogLevelButtonBuilder
    {
        private const string CycleGlyph = "↻"; // ↻ (U+21BB), hints that the button rotates

        private CheatSheetViewModel _viewModel;

        public LogLevelButtonBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public LogLevelButtonController Create()
        {
            var go = new GameObject("LogLevelButton", typeof(RectTransform));
            LogLevelButtonController controller = go.AddComponent<LogLevelButtonController>();
            controller.BindViewModel(_viewModel);

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

            var label = labelGo.AddComponent<Text>();
            label.font = HighLogic.UISkin.font;
            label.fontSize = SteamInputPalette.SettingsLogLevelFontSize;
            label.color = SteamInputPalette.SettingsLogLevelTextColor;
            label.alignment = TextAnchor.MiddleLeft;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;

            // Rotate glyph hinting the button cycles through the levels.
            var cycleGo = new GameObject("Cycle", typeof(RectTransform));
            cycleGo.transform.SetParent(go.transform, false);
            var cycle = cycleGo.AddComponent<Text>();
            cycle.text = CycleGlyph;
            cycle.font = HighLogic.UISkin.font;
            cycle.fontSize = SteamInputPalette.SettingsLogLevelCycleFontSize;
            cycle.color = SteamInputPalette.SettingsLogLevelCycleColor;
            cycle.alignment = TextAnchor.MiddleCenter;
            cycle.horizontalOverflow = HorizontalWrapMode.Overflow;
            cycle.verticalOverflow = VerticalWrapMode.Overflow;
            cycle.raycastTarget = false;

            controller.Bind(label);

            return controller;
        }

        public class LogLevelButtonController : BaseSteamInputController
        {
            // Levels in declaration order; the button cycles through them and wraps around.
            private static readonly LogLevel[] Levels = (LogLevel[]) Enum.GetValues(typeof(LogLevel));

            private Text _label;

            public void Bind(Text label)
            {
                this._label = label;
            }

            public void Start()
            {
                ViewModel?.OnLogLevelChanged.Add(OnLogLevelChanged);
                if (ViewModel != null)
                {
                    OnLogLevelChanged(ViewModel.LogLevel);
                }
            }

            public void OnDestroy()
            {
                ViewModel?.OnLogLevelChanged.Remove(OnLogLevelChanged);
            }

            /// <summary>Advance to the next level, wrapping back to the first one after the last.</summary>
            public void CycleLogLevel()
            {
                if (ViewModel == null)
                {
                    return;
                }
                int index = Array.IndexOf(Levels, ViewModel.LogLevel);
                LogLevel next = Levels[(index + 1) % Levels.Length];
                // Goes through SteamInputGlobalSettings, which fires OnLogLevelChanged back to us.
                ViewModel.LogLevel = next;
            }

            private void OnLogLevelChanged(LogLevel level)
            {
                if (_label != null)
                {
                    _label.text = GetLogLevelLabel(level);
                }
            }

            /// <summary>Localized level name, falling back to the enum name if no translation exists.</summary>
            private static string GetLogLevelLabel(LogLevel level)
            {
                string localized = ModLocalization.GetString("SteamInput_logLevel_" + level);
                return string.IsNullOrEmpty(localized) ? level.ToString() : localized;
            }
        }
    }
}
