using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.badge;
using com.github.lhervier.ksp.shared.ugui.styles;
using com.github.lhervier.ksp.shared;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.settings
{
    public class DiagnosticController : MonoBehaviour
    {
        private const string CtxDaemonSuffix = "CtxDaemon";

        private BadgeController _badge;

        private readonly List<GameObject> _contextRows = new List<GameObject>();

        // ===================================
        // Life cycle
        // ===================================

        private CheatSheetViewModel _viewModel;
        public DiagnosticController WithViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        private Transform _contextsBox;
        public DiagnosticController WithContextsBox(Transform contextsBox)
        {
            this._contextsBox = contextsBox;
            return this;
        }

        public DiagnosticController WithBadge(BadgeController badge)
        {
            this._badge = badge;
            return this;
        }

        public void Start()
        {
            if (_viewModel == null)
            {
                return;
            }
            _viewModel.OnGamepadConnected.Add(OnGamepadConnected);
            _viewModel.OnActivatedContextsChanged.Add(OnActivatedContextsChanged);
            OnGamepadConnected(_viewModel.GamepadConnected);
            OnActivatedContextsChanged(_viewModel.ActivatedContexts);
        }

        public void OnDestroy()
        {
            if (_viewModel == null)
            {
                return;
            }
            _viewModel.OnGamepadConnected.Remove(OnGamepadConnected);
            _viewModel.OnActivatedContextsChanged.Remove(OnActivatedContextsChanged);
        }

        // =============================================
        // Methods bound to events
        // =============================================

        private void OnGamepadConnected(bool connected)
        {
            if (_badge == null)
            {
                return;
            }
            string text = ModLocalization.GetString(connected ? "yes" : "no").ToUpperInvariant();
            if (connected)
            {
                _badge.SetState(text,
                    SteamInputPalette.SettingsBadgeOkTextColor,
                    SteamInputPalette.SettingsBadgeOkBgColor,
                    SteamInputPalette.SettingsBadgeOkBorderColor);
            }
            else
            {
                _badge.SetState(text,
                    SteamInputPalette.SettingsBadgeNoTextColor,
                    SteamInputPalette.SettingsBadgeNoBgColor,
                    SteamInputPalette.SettingsBadgeNoBorderColor);
            }
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

        // ============================================
        // Helpers
        // ============================================

        private GameObject BuildContextRow(string text)
        {
            var go = new GameObject("ContextRow", typeof(RectTransform));
            go.transform.SetParent(_contextsBox, false);
            var label = UGUILabels.AddLabel(go);
            label.text = "› " + text;
            label.fontSize = SteamInputPalette.SettingsContextFontSize;
            label.color = SteamInputPalette.SettingsContextRowColor;
            label.alignment = TextAlignmentOptions.Left;
            ApplyRowPadding(label);
            return go;
        }

        private GameObject BuildEmptyRow()
        {
            var go = new GameObject("ContextEmpty", typeof(RectTransform));
            go.transform.SetParent(_contextsBox, false);
            var label = UGUILabels.AddLabel(go);
            label.text = ModLocalization.GetString("settings_noContext");
            label.fontSize = SteamInputPalette.SettingsContextFontSize;
            label.fontStyle = FontStyles.Italic;
            label.color = SteamInputPalette.SettingsContextEmptyColor;
            label.alignment = TextAlignmentOptions.Left;
            ApplyRowPadding(label);
            return go;
        }

        // Horizontal inset is carried by the box's VLG padding; each row only fixes its height
        // so successive rows keep a small, even gap (mockup .kset-ctx-row vertical padding).
        private static void ApplyRowPadding(TextMeshProUGUI label)
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
