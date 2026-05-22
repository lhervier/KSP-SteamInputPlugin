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