using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.ui.styles;

namespace com.github.lhervier.ksp.ui.ugui
{
    public class TitleBarBuilder
    {
        private const float PaddingLeft = 8f;
        private const float PaddingRight = 8f;
        private const float PaddingTop = 5f;
        private const float PaddingBottom = 5f;
        private const float BottomBorderHeight = 1f;
        private static readonly Color TitleBarBottomBorder = new Color(68f / 255f, 68f / 255f, 68f / 255f);

        public static GameObject Create(string objectName)
        {
            var titleBarGo = new GameObject(objectName, typeof(RectTransform));

            var titleBarRect = titleBarGo.GetComponent<RectTransform>();
            titleBarRect.anchorMin = new Vector2(0f, 1f);
            titleBarRect.anchorMax = new Vector2(1f, 1f);
            titleBarRect.pivot = new Vector2(0.5f, 1f);
            titleBarRect.sizeDelta = new Vector2(0f, SteamInputPalette.TitleBarHeight);
            titleBarRect.anchoredPosition = Vector2.zero;

            // PopupDialog uses a VerticalLayoutGroup; without this the bar is stacked at the bottom.
            var titleBarLayout = titleBarGo.AddComponent<LayoutElement>();
            titleBarLayout.ignoreLayout = true;
            titleBarLayout.minHeight = SteamInputPalette.TitleBarHeight;
            titleBarLayout.preferredHeight = SteamInputPalette.TitleBarHeight;

            var headerImage = titleBarGo.AddComponent<Image>();
            headerImage.sprite = CheatSheetUGUIChrome.HeaderFillSprite;
            headerImage.type = Image.Type.Simple;
            headerImage.color = SteamInputPalette.Header;
            headerImage.raycastTarget = true;

            var bottomBorderGo = new GameObject("BottomBorder", typeof(RectTransform));
            bottomBorderGo.transform.SetParent(titleBarGo.transform, false);
            var bottomBorderRect = bottomBorderGo.GetComponent<RectTransform>();
            bottomBorderRect.anchorMin = new Vector2(0f, 0f);
            bottomBorderRect.anchorMax = new Vector2(1f, 0f);
            bottomBorderRect.pivot = new Vector2(0.5f, 0f);
            bottomBorderRect.sizeDelta = new Vector2(0f, BottomBorderHeight);
            bottomBorderRect.anchoredPosition = Vector2.zero;
            
            var bottomBorderImage = bottomBorderGo.AddComponent<Image>();
            bottomBorderImage.sprite = CheatSheetUGUIChrome.HeaderFillSprite;
            bottomBorderImage.type = Image.Type.Simple;
            bottomBorderImage.color = TitleBarBottomBorder;
            bottomBorderImage.raycastTarget = false;

            var rowGo = new GameObject("Row", typeof(RectTransform));
            rowGo.transform.SetParent(titleBarGo.transform, false);
            var rowRect = rowGo.GetComponent<RectTransform>();
            rowRect.anchorMin = Vector2.zero;
            rowRect.anchorMax = Vector2.one;
            rowRect.offsetMin = new Vector2(0f, BottomBorderHeight);
            rowRect.offsetMax = Vector2.zero;

            var layout = rowGo.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(
                Mathf.RoundToInt(PaddingLeft),
                Mathf.RoundToInt(PaddingRight),
                Mathf.RoundToInt(PaddingTop),
                Mathf.RoundToInt(PaddingBottom));
            layout.spacing = SteamInputPalette.IconTitleGap;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            var iconGo = new GameObject("GamepadIcon", typeof(RectTransform));
            iconGo.transform.SetParent(rowGo.transform, false);
            var iconLayout = iconGo.AddComponent<LayoutElement>();
            iconLayout.preferredWidth = SteamInputPalette.GamepadIconSize;
            iconLayout.preferredHeight = SteamInputPalette.GamepadIconSize;
            iconLayout.minWidth = SteamInputPalette.GamepadIconSize;
            iconLayout.minHeight = SteamInputPalette.GamepadIconSize;
            var iconImage = iconGo.AddComponent<Image>();
            iconImage.sprite = CheatSheetUGUIChrome.GamepadIconSprite;
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
                    SteamInputPalette.GamepadIconSize,
                    SteamInputPalette.GamepadIconSize);
            }

            var labelGo = new GameObject("TitleLabel", typeof(RectTransform));
            labelGo.transform.SetParent(rowGo.transform, false);
            var label = labelGo.AddComponent<Text>();
            label.text = ModLocalization.GetString("SteamInput_titleHelp").ToUpperInvariant();
            label.font = HighLogic.UISkin.font;
            label.fontSize = 12;
            label.fontStyle = FontStyle.Bold;
            label.color = SteamInputPalette.TitleText;
            label.alignment = TextAnchor.MiddleLeft;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;
            var labelLayout = labelGo.AddComponent<LayoutElement>();
            labelLayout.preferredHeight = SteamInputPalette.GamepadIconSize;

            return titleBarGo;
        }
    }
}