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
            // Main title bar object
            // -----------------------------------
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

            // Game object for the bottom border
            // ------------------------------------
            var bottomBorderGo = new GameObject("BottomBorder", typeof(RectTransform));
            bottomBorderGo.transform.SetParent(titleBarGo.transform, false);

            // Stretched horizontally, positionned at the bottom of the parent
            var bottomBorderRect = bottomBorderGo.GetComponent<RectTransform>();
            bottomBorderRect.anchorMin = new Vector2(0f, 0f);
            bottomBorderRect.anchorMax = new Vector2(1f, 0f);
            bottomBorderRect.pivot = new Vector2(0.5f, 0f);
            bottomBorderRect.sizeDelta = new Vector2(0f, SteamInputPalette.TitleBarSeparatorHeight);
            bottomBorderRect.anchoredPosition = Vector2.zero;
            
            // The separator
            var bottomBorderImage = bottomBorderGo.AddComponent<Image>();
            bottomBorderImage.sprite = SpritesGlobal.FillSprite;
            bottomBorderImage.type = Image.Type.Simple;
            bottomBorderImage.color = SteamInputPalette.TitleBarSeparatorColor;
            bottomBorderImage.raycastTarget = false;

            // Game object for the main row
            // -----------------------------------------
            var rowGo = new GameObject("Row", typeof(RectTransform));
            rowGo.transform.SetParent(titleBarGo.transform, false);

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

            // Icon Game Object
            // ---------------------------
            var iconGo = new GameObject("GamepadIcon", typeof(RectTransform));
            iconGo.transform.SetParent(rowGo.transform, false);

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

            // Windows title game object
            // -----------------------------
            var labelGo = new GameObject("TitleLabel", typeof(RectTransform));
            labelGo.transform.SetParent(rowGo.transform, false);
            
            // The label component
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

            return titleBarGo;
        }
    }
}