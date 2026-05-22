using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;

namespace com.github.lhervier.ksp.ui.ugui
{
    public class TitleBarBuilder
    {
        
        public static GameObject Create(string objectName)
        {
            var titleBarGo = new GameObject(objectName, typeof(RectTransform));

            // If the parent has a layout (and that's the case), forget about me, I will position elements myself.
            var titleBarLayout = titleBarGo.AddComponent<LayoutElement>();
            titleBarLayout.ignoreLayout = true;

            // Title bar zone relative to the parent, stretched horizontaly
            var titleBarRect = titleBarGo.GetComponent<RectTransform>();
            titleBarRect.anchorMin = new Vector2(0f, 1f);
            titleBarRect.anchorMax = new Vector2(1f, 1f);
            titleBarRect.pivot = new Vector2(0.5f, 1f);
            titleBarRect.sizeDelta = new Vector2(0f, SteamInputPalette.TitleBarHeight);
            titleBarRect.anchoredPosition = Vector2.zero;

            // Image for the backgroup of the title bar
            var headerImage = titleBarGo.AddComponent<Image>();
            headerImage.sprite = SpritesGlobal.FillSprite;
            headerImage.type = Image.Type.Simple;
            headerImage.color = SteamInputPalette.Header;
            headerImage.raycastTarget = false;

            GameObject separatorGo = CreateSeparator("BottomBorder");
            separatorGo.transform.SetParent(titleBarGo.transform, false);

            GameObject rootGo = CreateRoot("Root");
            rootGo.transform.SetParent(titleBarGo.transform, false);

            return titleBarGo;
        }

        public static GameObject CreateSeparator(string objectName)
        {
            var separatorGo = new GameObject(objectName, typeof(RectTransform));
            
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

        public static GameObject CreateRoot(string objectName)
        {
            var rootGo = new GameObject(objectName, typeof(RectTransform));

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

            var leftRow = CreateLeftRow("LeftRow");
            leftRow.transform.SetParent(rootGo.transform, false);

            var rightRow = CreateRightRow("RightRow");
            rightRow.transform.SetParent(rootGo.transform, false);

            return rootGo;
        }

        // ===================================================
        // Left row
        // ===================================================

        private static GameObject CreateLeftRow(string objectName)
        {
            var leftRowGo = new GameObject(objectName, typeof(RectTransform));

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

            var iconGo = CreateIcon("GamepadIcon");
            iconGo.transform.SetParent(leftRowLayoutElement.transform, false);

            var labelGo = CreateLabel("TitleLabel");
            labelGo.transform.SetParent(leftRowLayoutElement.transform, false);

            return leftRowGo;
        }

        private static GameObject CreateIcon(string objectName)
        {
            var iconGo = new GameObject(objectName, typeof(RectTransform));

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

        private static GameObject CreateLabel(string gameObject)
        {
            var labelGo = new GameObject(gameObject, typeof(RectTransform));
            
            var label = labelGo.AddComponent<Text>();
            label.text = ModLocalization.GetString("SteamInput_titleHelp").ToUpperInvariant();
            label.font = HighLogic.UISkin.font;
            label.fontSize = 12;
            label.fontStyle = FontStyle.Bold;
            label.color = SteamInputPalette.TitleBarTitleColor;
            label.alignment = TextAnchor.MiddleLeft;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;

            return labelGo;
        }

        // ====================================================
        // Right row
        // ====================================================

        private static GameObject CreateRightRow(string objectName)
        {
            var rightRowGo = new GameObject(objectName, typeof(RectTransform));

            // Horizontal layout containing the right-side placeholders, sized to their text content
            var rightRowLayout = rightRowGo.AddComponent<HorizontalLayoutGroup>();
            rightRowLayout.spacing = SteamInputPalette.DefaultSpacing;
            rightRowLayout.childAlignment = TextAnchor.MiddleLeft;
            rightRowLayout.childControlWidth = true;
            rightRowLayout.childControlHeight = true;
            rightRowLayout.childForceExpandWidth = false;
            rightRowLayout.childForceExpandHeight = false;

            var actionGroupGo = CreateActionGroupLabel("ActionGroup");
            actionGroupGo.transform.SetParent(rightRowGo.transform, false);

            var controllerGo = CreateGamepadNameLabel("Controller");
            controllerGo.transform.SetParent(rightRowGo.transform, false);

            var closeGo = CreateCloseButton("Close");
            closeGo.transform.SetParent(rightRowGo.transform, false);

            return rightRowGo;
        }

        private static GameObject CreateActionGroupLabel(string objectName)
        {
            var go = new GameObject(objectName, typeof(RectTransform));

            var label = go.AddComponent<Text>();
            label.text = "<action group>";
            label.font = HighLogic.UISkin.font;
            label.color = Color.white;
            label.raycastTarget = false;

            return go;
        }

        private static GameObject CreateGamepadNameLabel(string objectName)
        {
            var go = new GameObject(objectName, typeof(RectTransform));

            var label = go.AddComponent<Text>();
            label.text = "<gamepad>";
            label.font = HighLogic.UISkin.font;
            label.color = Color.white;
            label.raycastTarget = false;

            return go;
        }

        private static GameObject CreateCloseButton(string objectName)
        {
            var go = new GameObject(objectName, typeof(RectTransform));

            var label = go.AddComponent<Text>();
            label.text = "X";
            label.font = HighLogic.UISkin.font;
            label.color = Color.white;
            label.raycastTarget = false;

            return go;
        }
    }
}