using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using UnityEngine.Events;

namespace com.github.lhervier.ksp.ui.ugui.titleBar
{
    public class TitleBarBuilder
    {
        private CheatSheetViewModel _viewModel;
        private SeparatorBuilder _separatorBuilder;
        private LeftColumnBuilder _leftColumnBuilder;
        private ActionGroupLabelBuilder _actionGroupLabelBuilder;
        private GamepadLabelBuilder _gamepadLabelBuilder;
        private CloseButtonBuilder _closeButtonBuilder;

        public TitleBarBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._separatorBuilder = new SeparatorBuilder(viewModel);
            this._leftColumnBuilder = new LeftColumnBuilder(viewModel);
            this._actionGroupLabelBuilder = new ActionGroupLabelBuilder(viewModel);
            this._gamepadLabelBuilder = new GamepadLabelBuilder(viewModel);
            this._closeButtonBuilder = new CloseButtonBuilder(viewModel);
        }

        public GameObject CreateGameObject()
        {
            var titleBarGo = new GameObject("SteamInput.TitleBar", typeof(RectTransform));
            
            // If the parent has a layout (and that's the case), forget about me, I will position elements myself.
            var titleBarLayout = titleBarGo.AddComponent<LayoutElement>();
            titleBarLayout.ignoreLayout = true;

            // Title bar zone relative to the parent, stretched horizontaly
            // Beware not to overlap the borders
            var titleBarRect = titleBarGo.GetComponent<RectTransform>();
            titleBarRect.anchorMin = new Vector2(0f, 1f);
            titleBarRect.anchorMax = new Vector2(1f, 1f);
            titleBarRect.pivot = new Vector2(0.5f, 1f);
            titleBarRect.sizeDelta = new Vector2(-2f * SteamInputPalette.WindowBorderThickness, SteamInputPalette.TitleBarHeight);
            titleBarRect.anchoredPosition = new Vector2(0f, -SteamInputPalette.WindowBorderThickness);

            // Image for the backgroup of the title bar
            var headerImage = titleBarGo.AddComponent<Image>();
            headerImage.sprite = SpritesGlobal.FillSprite;
            headerImage.type = Image.Type.Simple;
            headerImage.color = SteamInputPalette.Header;
            headerImage.raycastTarget = false;

            GameObject separatorGo = _separatorBuilder.Create();
            separatorGo.transform.SetParent(titleBarGo.transform, false);

            // Menu and overlay are created BEFORE the root so the callbacks below can capture them.
            // They are parented LATER (after the root) to control sibling order = z-order:
            //   separator (bottom) → root → overlay → menu (top).
            GameObject menuGo = null;
            GameObject overlayGo = null;
            UnityAction toggleMenu = () => {
                bool willOpen = !menuGo.activeSelf;
                menuGo.SetActive(willOpen);
                overlayGo.SetActive(willOpen);
            };
            UnityAction closeMenu = () => {
                menuGo.SetActive(false);
                overlayGo.SetActive(false);
            };

            GameObject rootGo = CreateRoot(toggleMenu);
            rootGo.transform.SetParent(titleBarGo.transform, false);

            overlayGo = CreateOverlay(closeMenu);
            overlayGo.transform.SetParent(titleBarGo.transform, false);
            overlayGo.SetActive(false);

            menuGo = CreateMenu();
            menuGo.transform.SetParent(titleBarGo.transform, false);
            menuGo.SetActive(false);

            return titleBarGo;
        }

        public GameObject CreateRoot(UnityAction onMenuToggle)
        {
            var rootGo = new GameObject("SteamInput.TitleBar.Root", typeof(RectTransform));

            // Full size of the parent = the title bar, minus the bottom separator
            var rootRect = rootGo.GetComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = new Vector2(0f, SteamInputPalette.TitleBarSeparatorHeight);
            rootRect.offsetMax = Vector2.zero;

            // Horizontal layout splitting the title bar in two cells (left + right)
            var rootLayout = rootGo.AddComponent<HorizontalLayoutGroup>();
            rootLayout.padding = new RectOffset(
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingLeft),
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingRight),
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingTop),
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingBottom)
            );
            rootLayout.spacing = 0f;
            rootLayout.childAlignment = TextAnchor.MiddleLeft;
            // Width controlled by the layout so flexibleWidth on LeftRow pushes RightRow to the right
            // Height forced so the rows fill the title bar's height for proper vertical centering
            rootLayout.childControlWidth = true;
            rootLayout.childControlHeight = true;
            rootLayout.childForceExpandWidth = false;
            rootLayout.childForceExpandHeight = true;

            var leftRow = this._leftColumnBuilder.Create();
            leftRow.transform.SetParent(rootGo.transform, false);

            var rightRow = CreateRightRow(onMenuToggle);
            rightRow.transform.SetParent(rootGo.transform, false);

            return rootGo;
        }

        // ====================================================
        // Right row
        // ====================================================

        private GameObject CreateRightRow(UnityAction onMenuToggle)
        {
            var rightRowGo = new GameObject("SteamInput.TitleBar.RightColumn", typeof(RectTransform));

            // Horizontal layout containing the right-side placeholders, sized to their text content
            var rightRowLayout = rightRowGo.AddComponent<HorizontalLayoutGroup>();
            rightRowLayout.spacing = SteamInputPalette.DefaultSpacing;
            rightRowLayout.childAlignment = TextAnchor.MiddleLeft;
            rightRowLayout.childControlWidth = true;
            rightRowLayout.childControlHeight = true;
            rightRowLayout.childForceExpandWidth = false;
            rightRowLayout.childForceExpandHeight = false;

            var actionGroupGo = this._actionGroupLabelBuilder.Create();
            actionGroupGo.transform.SetParent(rightRowGo.transform, false);

            var controllerGo = this._gamepadLabelBuilder.Create();
            controllerGo.transform.SetParent(rightRowGo.transform, false);

            var menuGo = CreateMenuButton(onMenuToggle);
            menuGo.transform.SetParent(rightRowGo.transform, false);

            var closeGo = this._closeButtonBuilder.Create();
            closeGo.transform.SetParent(rightRowGo.transform, false);

            return rightRowGo;
        }

        private GameObject CreateMenuButton(UnityAction onClick)
        {
            var buttonGo = new GameObject("SteamInput.TitleBar.RightColumn.MenuButton", typeof(RectTransform));

            // Fixed square size; parent's HorizontalLayoutGroup has childControl* = true so it reads these
            var layoutElement = buttonGo.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = SteamInputPalette.DefaultButtonSize;
            layoutElement.preferredHeight = SteamInputPalette.DefaultButtonSize;
            layoutElement.minWidth = SteamInputPalette.DefaultButtonSize;
            layoutElement.minHeight = SteamInputPalette.DefaultButtonSize;

            // White background fill so the Button's color tint applies as-is (no multiplication)
            var image = buttonGo.AddComponent<Image>();
            image.sprite = SpritesGlobal.FillSprite;
            image.type = Image.Type.Simple;
            image.color = Color.white;
            image.raycastTarget = true;

            // Button: same color states as CloseButton (matches IMGUI's MenuButton which inherits CloseButton)
            var button = buttonGo.AddComponent<Button>();
            button.targetGraphic = image;
            var colors = button.colors;
            colors.normalColor = SteamInputPalette.Button;
            colors.highlightedColor = SteamInputPalette.ButtonHover;
            colors.pressedColor = SteamInputPalette.Button;
            colors.selectedColor = SteamInputPalette.Button;
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.1f;
            button.colors = colors;
            button.onClick.AddListener(onClick);

            // The "⋯" label (U+22EF), centered in the button
            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(buttonGo.transform, false);
            var labelRect = labelGo.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            var label = labelGo.AddComponent<Text>();
            label.text = "⋯";
            label.font = HighLogic.UISkin.font;
            label.fontSize = 13;
            label.color = SteamInputPalette.ButtonText;
            label.alignment = TextAnchor.MiddleCenter;
            label.raycastTarget = false;

            // Same text-color hover swap as CloseButton (ButtonText → white)
            var trigger = buttonGo.AddComponent<EventTrigger>();
            var enterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enterEntry.callback.AddListener(_ => label.color = Color.white);
            trigger.triggers.Add(enterEntry);
            var exitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            exitEntry.callback.AddListener(_ => label.color = SteamInputPalette.ButtonText);
            trigger.triggers.Add(exitEntry);

            return buttonGo;
        }

        private GameObject CreateMenu()
        {
            var menuGo = new GameObject("SteamInput.TitleBar.Menu", typeof(RectTransform));

            // Anchored at the title bar's bottom-right corner; offset inward by the title bar's
            // padding so the menu's top-right corner aligns with the menu button's bottom-right
            // (gives the "dropdown coming from the button" look from the mockup).
            var rect = menuGo.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(1f, 1f);
            rect.sizeDelta = new Vector2(ZonesMenuStyles.PanelWidth, ZonesMenuStyles.ContentPlaceholderHeight);
            rect.anchoredPosition = new Vector2(
                -SteamInputPalette.DefaultPaddingRight,
                SteamInputPalette.DefaultPaddingBottom
            );

            // Sliced chrome: dark background with a 1px border
            var image = menuGo.AddComponent<Image>();
            image.sprite = SpritesZonesMenu.ChromeSprite;
            image.type = Image.Type.Sliced;
            image.color = Color.white;
            // Catches clicks so they don't fall through to the overlay (which would close the menu)
            image.raycastTarget = true;

            return menuGo;
        }

        private GameObject CreateOverlay(UnityAction onClick)
        {
            var overlayGo = new GameObject("SteamInput.TitleBar.MenuOverlay", typeof(RectTransform));

            // Anchored to the title bar's bottom-center; oversized so it covers anything below the title bar
            // (popup body, and beyond if the popup is small relative to the screen).
            var rect = overlayGo.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(3000f, 3000f);
            rect.anchoredPosition = Vector2.zero;

            // Fully transparent but raycastTarget=true: invisible click trap
            var image = overlayGo.AddComponent<Image>();
            image.sprite = SpritesGlobal.FillSprite;
            image.type = Image.Type.Simple;
            image.color = new Color(0f, 0f, 0f, 0f);
            image.raycastTarget = true;

            // A click anywhere on the overlay closes the menu. Disable color transitions so the
            // overlay stays invisible during hover/press states.
            var button = overlayGo.AddComponent<Button>();
            button.targetGraphic = image;
            var colors = button.colors;
            colors.normalColor = new Color(1f, 1f, 1f, 0f);
            colors.highlightedColor = new Color(1f, 1f, 1f, 0f);
            colors.pressedColor = new Color(1f, 1f, 1f, 0f);
            colors.selectedColor = new Color(1f, 1f, 1f, 0f);
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0f;
            button.colors = colors;
            button.onClick.AddListener(onClick);

            return overlayGo;
        }
    }
}