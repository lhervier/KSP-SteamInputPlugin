using UnityEngine;
using UnityEngine.UI;
using TMPro;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui.styles;
using com.github.lhervier.ksp.shared.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui.button;
using com.github.lhervier.ksp.shared.ugui.overlay;
using com.github.lhervier.ksp.shared.ugui;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.selector
{
    /// <summary>
    /// Config picker shown at the top of the body: a combobox listing the available configs
    /// (title + controller type, plus a "none" entry) and a refresh button to rescan the folder.
    /// The list comes from the ViewModel; selecting an entry sets the controller config name.
    /// </summary>
    public class SelectorBuilder : IUGUIBuilder<SelectorController>
    {
        // Preferred glyphs with degraded alternatives: the game's TMP font atlases (base +
        // fallbacks) do not necessarily contain them; the actual glyph is picked at build time
        // through DefaultPalette.PickGlyph. "↓"/"↑" are known to be available.
        private static string CaretGlyph => DefaultPalette.PickGlyph("▼", "▾", "↓");
        private static string RefreshGlyph => DefaultPalette.PickGlyph("↻", "⟳", "↺", "↓↑");

        // ==================================
        // Builder parameters
        // ==================================

        private CheatSheetViewModel _viewModel;
        public SelectorBuilder ViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        // =================================
        // Build
        // =================================

        public SelectorController Build()
        {
            var go = new GameObject("SteamInput.Body.Selector", typeof(RectTransform));
            SelectorController controller = go.AddComponent<SelectorController>();
            controller.ViewModel(_viewModel);

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
            // combo height so the icon reads at a glance instead of getting lost in a tiny chip.
            // Sprite tag (no circular-arrow glyph in the game fonts), text glyph as a fallback
            // if the texture is missing.
            ButtonController refresh = new ButtonBuilder()
                .ObjectName("Refresh")
                .Label(SpritesIcons.HasSprite("refresh") ? "<sprite name=\"refresh\" tint=1>" : RefreshGlyph)
                .Build();
            refresh.transform.SetParent(go.transform, false);
            controller.RefreshButton(refresh);

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

            var label = labelGo.AddComponent<TextMeshProUGUI>();
            label.font = DefaultPalette.Font;
            label.fontSize = SteamInputPalette.ComboFontSize;
            label.color = SteamInputPalette.ComboTextColor;
            label.alignment = TextAlignmentOptions.Left;
            label.enableWordWrapping = false; // clipped by the RectMask2D above
            label.overflowMode = TextOverflowModes.Overflow;
            label.raycastTarget = false;
            controller.Label(label);

            // Caret
            var caretGo = new GameObject("Caret", typeof(RectTransform));
            caretGo.transform.SetParent(comboGo.transform, false);
            var caret = caretGo.AddComponent<TextMeshProUGUI>();
            caret.text = CaretGlyph;
            caret.font = DefaultPalette.Font;
            caret.fontSize = SteamInputPalette.ComboCaretFontSize;
            caret.color = SteamInputPalette.ComboCaretColor;
            caret.alignment = TextAlignmentOptions.Center;
            caret.enableWordWrapping = false;
            caret.overflowMode = TextOverflowModes.Overflow;
            caret.raycastTarget = false;

            // The overlay and dropdown are built detached; they are moved next to the popup root and
            // positioned under the combo when opened (see SelectorController.OpenDropdown).
            OverlayController overlayController = new OverlayBuilder().Build();
            overlayController.gameObject.SetActive(false);
            controller.OverlayController(overlayController);

            GameObject dropdown = BuildDropdown(out RectTransform content);
            controller.Content(content);
            controller.Dropdown(dropdown);

            controller.ComboRect(comboGo.GetComponent<RectTransform>());
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
    }
}
