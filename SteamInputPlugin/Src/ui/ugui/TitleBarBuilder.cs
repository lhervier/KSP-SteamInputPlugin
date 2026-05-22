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
            var rowGo = CreateLeftRow("LeftRow");

            var iconGo = CreateIcon("GamepadIcon");
            iconGo.transform.SetParent(rowGo.transform, false);

            var labelGo = CreateLabel("TitleLabel");
            labelGo.transform.SetParent(rowGo.transform, false);
            
            return rowGo;
        }

        private static GameObject CreateLeftRow(string objectName)
        {
            var rowGo = new GameObject(objectName, typeof(RectTransform));

            // Full size of the parent = the title bar, minus the bottom separator
            var rowRect = rowGo.GetComponent<RectTransform>();
            rowRect.anchorMin = Vector2.zero;
            rowRect.anchorMax = Vector2.one;
            rowRect.offsetMin = new Vector2(0f, SteamInputPalette.TitleBarSeparatorHeight);
            rowRect.offsetMax = Vector2.zero;

            // Horizontal layout with padding
            var layout = rowGo.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingLeft),
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingRight),
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingTop),
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingBottom)
            );
            layout.spacing = SteamInputPalette.DefaultSpacing;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            return rowGo;
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
    }
}