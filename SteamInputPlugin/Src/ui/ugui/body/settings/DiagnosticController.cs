using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.shared;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.settings
{
    public class DiagnosticController : MonoBehaviour
    {
        private const string CtxDaemonSuffix = "CtxDaemon";

        private Image _badgeImage;
        private Text _badgeText;
        private Sprite _badgeOkSprite;
        private Sprite _badgeNoSprite;

        private readonly List<GameObject> _contextRows = new List<GameObject>();

        // ===================================
        // Life cycle
        // ===================================

        private CheatSheetViewModel _viewModel;
        public DiagnosticController ViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        private Transform _contextsBox;
        public DiagnosticController ContextsBox(Transform contextsBox)
        {
            this._contextsBox = contextsBox;
            return this;
        }

        public DiagnosticController Badge(Image image, Text text, Sprite okSprite, Sprite noSprite)
        {
            this._badgeImage = image;
            this._badgeText = text;
            this._badgeOkSprite = okSprite;
            this._badgeNoSprite = noSprite;
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

        // ============================================
        // Helpers
        // ============================================

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
