using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.steaminput.ui.model;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.shared.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui.button;
using com.github.lhervier.ksp.shared.ugui.popup;
using com.github.lhervier.ksp.shared.ugui.overlay;
using com.github.lhervier.ksp.shared;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.selector
{
    public class SelectorController : BaseSteamInputController
    {
        private Transform _popupWindow;
        private readonly List<GameObject> _items = new List<GameObject>();
        private List<UIGamepadConfig> _configs = new List<UIGamepadConfig>();

        // ===================================
        // Life Cycle
        // ===================================
        
        private Text _label;
        public SelectorController Label(Text label)
        {
            this._label = label;
            return this;
        }

        private RectTransform _comboRect;
        public SelectorController ComboRect(RectTransform comboRect)
        {
            this._comboRect = comboRect;
            return this;
        }

        private GameObject _dropdown;
        private RectTransform _dropdownRect;
        public SelectorController Dropdown(GameObject dropdown)
        {
            this._dropdown = dropdown;
            this._dropdownRect = dropdown.GetComponent<RectTransform>();
            return this;
        }

        private RectTransform _content;
        public SelectorController Content(RectTransform content)
        {
            this._content = content;
            return this;
        }

        private OverlayController _overlayController;
        public SelectorController OverlayController(OverlayController overlayController)
        {
            this._overlayController = overlayController;
            return this;
        }

        private ButtonController _buttonController;
        public SelectorController RefreshButton(ButtonController buttonController)
        {
            this._buttonController = buttonController;
            return this;
        }

        public void Start()
        {
            // The dropdown/overlay are moved to the popup window on open, so they escape the body's
            // scroll mask and draw on top of everything (and die with the popup). The window carries
            // the PopupController, so we resolve it by component rather than by walking names.
            var popupController = GetComponentInParent<PopupController>();
            _popupWindow = (popupController != null) ? popupController.transform : null;

            this.ViewModel.OnConfigsChanged.Add(OnConfigsChanged);
            this.ViewModel.OnGamepadConfigNameChanged.Add(OnConfigNameChanged);
            OnConfigsChanged(this.ViewModel.Configs);

            if( this._buttonController != null )
            {
                this._buttonController.OnClick.Add(ViewModel.RefreshConfigs);
            }

            if( this._overlayController != null )
            {
                this._overlayController.OnClose.Add(CloseDropdown);
            }
        }

        public void OnDestroy()
        {
            if( this._overlayController != null )
            {
                this._overlayController.OnClose.Remove(CloseDropdown);
            }
            if( this._buttonController != null )
            {
                this._buttonController.OnClick.Remove(ViewModel.RefreshConfigs);
            }
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
                _overlayController.gameObject.transform.SetParent(_popupWindow, false);
                _dropdown.transform.SetParent(_popupWindow, false);
                _overlayController.gameObject.transform.SetAsLastSibling();
                _dropdown.transform.SetAsLastSibling();
            }

            _overlayController.gameObject.SetActive(true);
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
            if (_overlayController?.gameObject != null) _overlayController.gameObject.SetActive(false);
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
