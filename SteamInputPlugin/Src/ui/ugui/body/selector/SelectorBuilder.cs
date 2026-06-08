using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.steaminput.ui.model;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui.styles;
using com.github.lhervier.ksp.shared.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui.button;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.selector
{
    /// <summary>
    /// Config picker shown at the top of the body: a combobox listing the available configs
    /// (title + controller type, plus a "none" entry) and a refresh button to rescan the folder.
    /// The list comes from the ViewModel; selecting an entry sets the controller config name.
    /// </summary>
    public class SelectorBuilder
    {
        private const string CaretGlyph = "▼";    // ▼ (U+25BC, renders like the menu order arrows)
        private const string RefreshGlyph = "↻";  // ↻ (U+21BB)

        private CheatSheetViewModel _viewModel;
        private ButtonBuilder _buttonBuilder;
        private OverlayBuilder _overlayBuilder;

        public SelectorBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._buttonBuilder = new ButtonBuilder();
            this._overlayBuilder = new OverlayBuilder(viewModel);
        }

        public SelectorController Create()
        {
            var go = new GameObject("SteamInput.Body.Selector", typeof(RectTransform));
            SelectorController controller = go.AddComponent<SelectorController>();
            controller.Initialize(_viewModel);

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(
                Mathf.RoundToInt(DefaultPalette.PaddingLeft),
                Mathf.RoundToInt(DefaultPalette.PaddingRight),
                Mathf.RoundToInt(SteamInputPalette.ComboPaddingV),
                Mathf.RoundToInt(SteamInputPalette.ComboPaddingV));
            layout.spacing = DefaultPalette.Spacing;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            BuildCombo(go.transform, controller);

            // Refresh button: triggers a rescan of the Steam config folder. Sized to match the
            // combo height so the ↻ glyph reads at a glance instead of getting lost in a tiny chip.
            ButtonController refresh = _buttonBuilder.Create(
                "Refresh",
                RefreshGlyph,
                () => _viewModel.RefreshConfigs());
            refresh.transform.SetParent(go.transform, false);

            var refreshLayout = refresh.GetComponent<LayoutElement>();
            refreshLayout.minWidth = SteamInputPalette.ComboHeight;
            refreshLayout.minHeight = SteamInputPalette.ComboHeight;
            refreshLayout.preferredWidth = SteamInputPalette.ComboHeight;
            refreshLayout.preferredHeight = SteamInputPalette.ComboHeight;

            return controller;
        }

        // The combobox: a fixed-height, flexible-width button (current selection + caret).
        private void BuildCombo(Transform parent, SelectorController controller)
        {
            var comboGo = new GameObject("Combo", typeof(RectTransform));
            comboGo.transform.SetParent(parent, false);

            var comboElement = comboGo.AddComponent<LayoutElement>();
            comboElement.flexibleWidth = 1f;
            comboElement.minHeight = SteamInputPalette.ComboHeight;
            comboElement.preferredHeight = SteamInputPalette.ComboHeight;

            var comboImage = comboGo.AddComponent<Image>();
            comboImage.sprite = SpritesZones.ComboSprite; // field fill + 1px border
            comboImage.type = Image.Type.Sliced;
            comboImage.color = Color.white;
            comboImage.raycastTarget = true;

            var comboButton = comboGo.AddComponent<Button>();
            comboButton.targetGraphic = comboImage;
            comboButton.transition = Selectable.Transition.None;
            comboButton.onClick.AddListener(() => controller.ToggleDropdown());

            var comboLayout = comboGo.AddComponent<HorizontalLayoutGroup>();
            comboLayout.padding = new RectOffset(
                Mathf.RoundToInt(SteamInputPalette.ComboPaddingH),
                Mathf.RoundToInt(SteamInputPalette.ComboPaddingH),
                0, 0);
            comboLayout.spacing = DefaultPalette.Spacing;
            comboLayout.childAlignment = TextAnchor.MiddleLeft;
            comboLayout.childControlWidth = true;
            comboLayout.childControlHeight = true;
            comboLayout.childForceExpandWidth = false;
            comboLayout.childForceExpandHeight = true;

            // Label clipped to its width: long titles are truncated rather than overflowing.
            var clipGo = new GameObject("LabelClip", typeof(RectTransform));
            clipGo.transform.SetParent(comboGo.transform, false);
            clipGo.AddComponent<RectMask2D>();
            var clipElement = clipGo.AddComponent<LayoutElement>();
            clipElement.flexibleWidth = 1f;

            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(clipGo.transform, false);
            var labelRect = labelGo.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            var label = labelGo.AddComponent<Text>();
            label.font = HighLogic.UISkin.font;
            label.fontSize = SteamInputPalette.ComboFontSize;
            label.color = SteamInputPalette.ComboTextColor;
            label.alignment = TextAnchor.MiddleLeft;
            label.horizontalOverflow = HorizontalWrapMode.Overflow; // clipped by the RectMask2D above
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;

            // Caret
            var caretGo = new GameObject("Caret", typeof(RectTransform));
            caretGo.transform.SetParent(comboGo.transform, false);
            var caret = caretGo.AddComponent<Text>();
            caret.text = CaretGlyph;
            caret.font = HighLogic.UISkin.font;
            caret.fontSize = SteamInputPalette.ComboCaretFontSize;
            caret.color = SteamInputPalette.ComboCaretColor;
            caret.alignment = TextAnchor.MiddleCenter;
            caret.horizontalOverflow = HorizontalWrapMode.Overflow;
            caret.verticalOverflow = VerticalWrapMode.Overflow;
            caret.raycastTarget = false;

            // The overlay and dropdown are built detached; they are moved next to the popup root and
            // positioned under the combo when opened (see SelectorController.OpenDropdown).
            GameObject overlay = _overlayBuilder.Create(() => controller.CloseDropdown()).gameObject;
            overlay.SetActive(false);

            RectTransform content;
            GameObject dropdown = BuildDropdown(out content);

            controller.Bind(label, comboGo.GetComponent<RectTransform>(), dropdown, content, overlay);
        }

        // Scrollable dropdown panel. Built detached and hidden; positioned/parented on open.
        private GameObject BuildDropdown(out RectTransform content)
        {
            var dropdownGo = new GameObject("SteamInput.Body.Selector.Dropdown", typeof(RectTransform));

            // The popup window stacks its children with a VerticalLayoutGroup; opt out so our
            // anchors/position (set on open) take effect instead of being laid out at the bottom.
            var dropdownElement = dropdownGo.AddComponent<LayoutElement>();
            dropdownElement.ignoreLayout = true;

            var image = dropdownGo.AddComponent<Image>();
            image.sprite = SpritesZonesMenu.ChromeSprite; // dark fill + 1px border (same as the zones menu)
            image.type = Image.Type.Sliced;
            image.color = Color.white;
            image.raycastTarget = true;

            var scrollRect = dropdownGo.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 20f;

            // Viewport clips the scrolled content.
            var viewportGo = new GameObject("Viewport", typeof(RectTransform));
            viewportGo.transform.SetParent(dropdownGo.transform, false);
            var viewportRect = viewportGo.GetComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;
            viewportGo.AddComponent<RectMask2D>();
            var viewportImage = viewportGo.AddComponent<Image>();
            viewportImage.sprite = SpritesGlobal.FillSprite;
            viewportImage.type = Image.Type.Simple;
            viewportImage.color = Color.clear;
            viewportImage.raycastTarget = true;
            scrollRect.viewport = viewportRect;

            // Content: stacks the items, height driven by the ContentSizeFitter.
            var contentGo = new GameObject("Content", typeof(RectTransform));
            contentGo.transform.SetParent(viewportGo.transform, false);
            var contentRect = contentGo.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = Vector2.zero;
            scrollRect.content = contentRect;

            var layout = contentGo.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(0, 0,
                Mathf.RoundToInt(SteamInputPalette.MenuPaddingTop),
                Mathf.RoundToInt(SteamInputPalette.MenuPaddingBottom));
            layout.spacing = 0f;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var fitter = contentGo.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            dropdownGo.SetActive(false);
            content = contentRect;
            return dropdownGo;
        }

        public class SelectorController : BaseSteamInputController
        {
            private Text _label;
            private RectTransform _comboRect;
            private GameObject _dropdown;
            private RectTransform _dropdownRect;
            private RectTransform _content;
            private GameObject _overlay;
            private Transform _popupWindow;
            private readonly List<GameObject> _items = new List<GameObject>();
            private List<UIGamepadConfig> _configs = new List<UIGamepadConfig>();

            public void Bind(Text label, RectTransform comboRect, GameObject dropdown, RectTransform content, GameObject overlay)
            {
                this._label = label;
                this._comboRect = comboRect;
                this._dropdown = dropdown;
                this._dropdownRect = dropdown.GetComponent<RectTransform>();
                this._content = content;
                this._overlay = overlay;
            }

            public void Start()
            {
                // The popup window is the body's parent; the dropdown/overlay are moved here on open
                // so they escape the body's scroll mask and draw on top (and die with the popup).
                Transform t = transform;
                while (t != null && t.name != BodyBuilder.BODY_NAME)
                {
                    t = t.parent;
                }
                _popupWindow = (t != null) ? t.parent : null;

                this.ViewModel.OnConfigsChanged.Add(OnConfigsChanged);
                this.ViewModel.OnGamepadConfigNameChanged.Add(OnConfigNameChanged);
                OnConfigsChanged(this.ViewModel.Configs);
            }

            public void OnDestroy()
            {
                this.ViewModel?.OnConfigsChanged.Remove(OnConfigsChanged);
                this.ViewModel?.OnGamepadConfigNameChanged.Remove(OnConfigNameChanged);
            }

            public void ToggleDropdown()
            {
                if (_dropdown == null)
                {
                    return;
                }
                if (_dropdown.activeSelf)
                {
                    CloseDropdown();
                }
                else
                {
                    OpenDropdown();
                }
            }

            private void OpenDropdown()
            {
                // Move under the popup root (out of the body's scroll mask), overlay below the dropdown.
                if (_popupWindow != null)
                {
                    _overlay.transform.SetParent(_popupWindow, false);
                    _dropdown.transform.SetParent(_popupWindow, false);
                    _overlay.transform.SetAsLastSibling();
                    _dropdown.transform.SetAsLastSibling();
                }

                _overlay.SetActive(true);
                _dropdown.SetActive(true);

                // Size to the content, capped so a long list scrolls instead of overflowing.
                LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
                float height = Mathf.Min(LayoutUtility.GetPreferredHeight(_content), SteamInputPalette.ComboDropdownMaxHeight);

                // Place the panel just under the combo, matching its width.
                var corners = new Vector3[4];
                _comboRect.GetWorldCorners(corners); // 0=bottom-left, 1=top-left, 2=top-right, 3=bottom-right
                _dropdownRect.anchorMin = new Vector2(0f, 1f);
                _dropdownRect.anchorMax = new Vector2(0f, 1f);
                _dropdownRect.pivot = new Vector2(0f, 1f);
                _dropdownRect.sizeDelta = new Vector2(_comboRect.rect.width, height);
                _dropdownRect.position = corners[0];
            }

            public void CloseDropdown()
            {
                if (_dropdown != null) _dropdown.SetActive(false);
                if (_overlay != null) _overlay.SetActive(false);
            }

            private void OnConfigsChanged(List<UIGamepadConfig> configs)
            {
                _configs = configs ?? new List<UIGamepadConfig>();
                RebuildItems();
                UpdateLabel(this.ViewModel.GamepadConfigName);
            }

            private void OnConfigNameChanged(string name)
            {
                UpdateLabel(name);
                RebuildItems();
            }

            // Shows the selected config's title (display), or the "none" placeholder when empty.
            private void UpdateLabel(string name)
            {
                if (_label == null)
                {
                    return;
                }
                if (string.IsNullOrEmpty(name))
                {
                    _label.text = ModLocalization.GetString("SteamInput_configNone");
                    return;
                }
                foreach (UIGamepadConfig config in _configs)
                {
                    if (config.Name == name)
                    {
                        _label.text = config.Title;
                        return;
                    }
                }
                _label.text = name; // selected config not in the current list (e.g. file removed)
            }

            private void RebuildItems()
            {
                if (_content == null)
                {
                    return;
                }
                foreach (GameObject item in _items)
                {
                    Destroy(item);
                }
                _items.Clear();

                string selected = this.ViewModel.GamepadConfigName;

                // ‹none› entry first.
                _items.Add(BuildItem(
                    ModLocalization.GetString("SteamInput_configNone"),
                    null,
                    "",
                    string.IsNullOrEmpty(selected)));

                foreach (UIGamepadConfig config in _configs)
                {
                    _items.Add(BuildItem(config.Title, config.ControllerLabel, config.Name, config.Name == selected));
                }
            }

            private GameObject BuildItem(string title, string typeLabel, string name, bool selected)
            {
                var itemGo = new GameObject("ConfigItem", typeof(RectTransform));
                itemGo.transform.SetParent(_content, false);

                var image = itemGo.AddComponent<Image>();
                image.sprite = SpritesGlobal.FillSprite;
                image.type = Image.Type.Simple;
                // Opaque white base: the color tint multiplies the CanvasRenderer color into the
                // vertex color, so a clear base would zero out the alpha and the hover would never show.
                image.color = Color.white;
                image.raycastTarget = true;

                // Hover feedback via the Button color tint (transparent -> dark on hover).
                var button = itemGo.AddComponent<Button>();
                button.targetGraphic = image;
                var colors = button.colors;
                colors.normalColor = Color.clear;
                colors.highlightedColor = SteamInputPalette.ComboItemHoverColor;
                colors.pressedColor = SteamInputPalette.ComboItemHoverColor;
                colors.selectedColor = Color.clear;
                colors.disabledColor = Color.clear;
                colors.colorMultiplier = 1f;
                colors.fadeDuration = 0.1f;
                button.colors = colors;
                button.onClick.AddListener(() =>
                {
                    this.ViewModel.GamepadConfigName = name;
                    CloseDropdown();
                });

                var layout = itemGo.AddComponent<VerticalLayoutGroup>();
                layout.padding = new RectOffset(
                    Mathf.RoundToInt(SteamInputPalette.ComboItemPaddingH),
                    Mathf.RoundToInt(SteamInputPalette.ComboItemPaddingH),
                    Mathf.RoundToInt(SteamInputPalette.ComboItemPaddingV),
                    Mathf.RoundToInt(SteamInputPalette.ComboItemPaddingV));
                layout.spacing = 1f;
                layout.childAlignment = TextAnchor.MiddleLeft;
                layout.childControlWidth = true;
                layout.childControlHeight = true;
                layout.childForceExpandWidth = true;
                layout.childForceExpandHeight = false;

                bool isNone = string.IsNullOrEmpty(name);

                var titleGo = new GameObject("Title", typeof(RectTransform));
                titleGo.transform.SetParent(itemGo.transform, false);
                var titleText = titleGo.AddComponent<Text>();
                titleText.text = title;
                titleText.font = HighLogic.UISkin.font;
                titleText.fontSize = SteamInputPalette.ComboItemTitleFontSize;
                titleText.fontStyle = isNone ? FontStyle.Italic : FontStyle.Normal;
                titleText.color = selected
                    ? SteamInputPalette.ComboItemTitleSelectedColor
                    : (isNone ? SteamInputPalette.ComboItemNoneColor : SteamInputPalette.ComboItemTitleColor);
                titleText.alignment = TextAnchor.MiddleLeft;
                titleText.horizontalOverflow = HorizontalWrapMode.Overflow;
                titleText.verticalOverflow = VerticalWrapMode.Overflow;
                titleText.raycastTarget = false;

                if (!string.IsNullOrEmpty(typeLabel))
                {
                    var typeGo = new GameObject("Type", typeof(RectTransform));
                    typeGo.transform.SetParent(itemGo.transform, false);
                    var typeText = typeGo.AddComponent<Text>();
                    typeText.text = typeLabel.ToUpperInvariant();
                    typeText.font = HighLogic.UISkin.font;
                    typeText.fontSize = SteamInputPalette.ComboItemTypeFontSize;
                    typeText.color = SteamInputPalette.ComboItemTypeColor;
                    typeText.alignment = TextAnchor.MiddleLeft;
                    typeText.horizontalOverflow = HorizontalWrapMode.Overflow;
                    typeText.verticalOverflow = VerticalWrapMode.Overflow;
                    typeText.raycastTarget = false;
                }

                return itemGo;
            }
        }
    }
}
