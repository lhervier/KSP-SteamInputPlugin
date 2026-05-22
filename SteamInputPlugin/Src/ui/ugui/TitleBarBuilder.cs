using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using UnityEngine.Events;

namespace com.github.lhervier.ksp.ui.ugui
{
    public class TitleBarBuilder
    {
        private CheatSheetViewModel _viewModel;

        public TitleBarBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
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

            GameObject separatorGo = CreateSeparator();
            separatorGo.transform.SetParent(titleBarGo.transform, false);

            GameObject rootGo = CreateRoot();
            rootGo.transform.SetParent(titleBarGo.transform, false);

            return titleBarGo;
        }

        public GameObject CreateSeparator()
        {
            var separatorGo = new GameObject("SteamInput.TitleBar.Separator", typeof(RectTransform));
            
            // Stretched horizontally, positionned at the bottom of the parent
            var separatorRect = separatorGo.GetComponent<RectTransform>();
            separatorRect.anchorMin = new Vector2(0f, 0f);
            separatorRect.anchorMax = new Vector2(1f, 0f);
            separatorRect.pivot = new Vector2(0.5f, 0f);
            separatorRect.sizeDelta = new Vector2(0f, SteamInputPalette.TitleBarSeparatorHeight);
            separatorRect.anchoredPosition = Vector2.zero;
            
            // The separator
            var separatorImage = separatorGo.AddComponent<Image>();
            separatorImage.sprite = SpritesGlobal.FillSprite;
            separatorImage.type = Image.Type.Simple;
            separatorImage.color = SteamInputPalette.TitleBarSeparatorColor;
            separatorImage.raycastTarget = false;

            return separatorGo;
        }

        public GameObject CreateRoot()
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

            var leftRow = CreateLeftColumn();
            leftRow.transform.SetParent(rootGo.transform, false);

            var rightRow = CreateRightRow();
            rightRow.transform.SetParent(rootGo.transform, false);

            return rootGo;
        }

        // ===================================================
        // Left row
        // ===================================================

        private GameObject CreateLeftColumn()
        {
            var leftRowGo = new GameObject("SteamInput.TitleBar.LeftColumn", typeof(RectTransform));

            // Greedy on width so it consumes the leftover space and pushes the right row against the right edge
            var leftRowLayoutElement = leftRowGo.AddComponent<LayoutElement>();
            leftRowLayoutElement.flexibleWidth = 1f;

            // Horizontal layout containing icon + label
            var leftRowLayout = leftRowGo.AddComponent<HorizontalLayoutGroup>();
            leftRowLayout.spacing = SteamInputPalette.DefaultSpacing;
            leftRowLayout.childAlignment = TextAnchor.MiddleLeft;
            leftRowLayout.childControlWidth = false;
            leftRowLayout.childControlHeight = false;
            leftRowLayout.childForceExpandWidth = false;
            leftRowLayout.childForceExpandHeight = false;

            var iconGo = CreateIcon();
            iconGo.transform.SetParent(leftRowLayoutElement.transform, false);

            var labelGo = CreateLabel();
            labelGo.transform.SetParent(leftRowLayoutElement.transform, false);

            return leftRowGo;
        }

        private GameObject CreateIcon()
        {
            var iconGo = new GameObject("SteamInput.TitleBar.LeftColumn.Icon", typeof(RectTransform));

            // The icon itself
            var iconImage = iconGo.AddComponent<Image>();
            iconImage.sprite = SpritesTitleBar.GamepadIconSprite;
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = false;
            if (iconImage.sprite == null)
            {
                iconGo.SetActive(false);
            }
            else
            {
                var iconRect = iconGo.GetComponent<RectTransform>();
                iconRect.sizeDelta = new Vector2(
                    SteamInputPalette.DefaultIconSize,
                    SteamInputPalette.DefaultIconSize
                );
            }
            return iconGo;
        }

        private GameObject CreateLabel()
        {
            var labelGo = new GameObject("SteamInput.TitleBar.LeftColumn.Label", typeof(RectTransform));
            
            var label = labelGo.AddComponent<Text>();
            label.text = ModLocalization.GetString("SteamInput_titleHelp").ToUpperInvariant();
            label.font = HighLogic.UISkin.font;
            label.fontSize = 12;
            label.fontStyle = FontStyle.Bold;
            label.color = SteamInputPalette.TitleBarLabelColor;
            label.alignment = TextAnchor.MiddleLeft;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;

            return labelGo;
        }

        // ====================================================
        // Right row
        // ====================================================

        private GameObject CreateRightRow()
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

            var actionGroupGo = CreateActionGroupLabel();
            actionGroupGo.transform.SetParent(rightRowGo.transform, false);

            var controllerGo = CreateGamepadNameLabel();
            controllerGo.transform.SetParent(rightRowGo.transform, false);

            var closeGo = CreateCloseButton();
            closeGo.transform.SetParent(rightRowGo.transform, false);

            return rightRowGo;
        }

        private GameObject CreateActionGroupLabel()
        {
            var badgeGo = new GameObject("SteamInput.TitleBar.RightColumn.ActionGroup", typeof(RectTransform));

            // Sliced sprite: transparent fill with a green border
            var image = badgeGo.AddComponent<Image>();
            image.sprite = SpritesTitleBar.ActionGroupBorderSprite;
            image.type = Image.Type.Sliced;
            image.color = Color.white;
            image.raycastTarget = false;

            // Padding around the text; badge size driven by content + padding via the HLG's reported preferredSize
            var layout = badgeGo.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(5, 5, 2, 2);
            layout.spacing = 0f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            // Green label
            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(badgeGo.transform, false);

            var label = labelGo.AddComponent<Text>();
            label.text = "<action group>";
            label.font = HighLogic.UISkin.font;
            label.fontSize = 10;
            label.color = SteamInputPalette.TitleBarActionGroupLabelColor;
            label.alignment = TextAnchor.MiddleCenter;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;

            return badgeGo;
        }

        private GameObject CreateGamepadNameLabel()
        {
            var go = new GameObject("SteamInput.TitleBar.RightColumn.GamepadName", typeof(RectTransform));

            var label = go.AddComponent<Text>();
            label.text = "<gamepad>";
            label.font = HighLogic.UISkin.font;
            label.color = Color.white;
            label.raycastTarget = false;

            return go;
        }

        private GameObject CreateCloseButton()
        {
            var buttonGo = new GameObject("SteamInput.TitleBar.RightColumn.Close", typeof(RectTransform));

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

            // Button: hover/press color transitions on the background, plus the click handler
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
            button.onClick.AddListener(() => this._viewModel.CloseWindow());

            // The "X" label, centered in the button
            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(buttonGo.transform, false);
            var labelRect = labelGo.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            var label = labelGo.AddComponent<Text>();
            label.text = "×";
            label.font = HighLogic.UISkin.font;
            label.fontSize = 13;
            label.color = SteamInputPalette.ButtonText;
            label.alignment = TextAnchor.MiddleCenter;
            label.raycastTarget = false;

            // Button.colors only tints the targetGraphic (the background); replicate IMGUI's text
            // color swap (ButtonText → white on hover) via an EventTrigger on the same GameObject.
            var trigger = buttonGo.AddComponent<EventTrigger>();
            var enterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enterEntry.callback.AddListener(_ => label.color = Color.white);
            trigger.triggers.Add(enterEntry);
            var exitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            exitEntry.callback.AddListener(_ => label.color = SteamInputPalette.ButtonText);
            trigger.triggers.Add(exitEntry);

            return buttonGo;
        }
    }
}