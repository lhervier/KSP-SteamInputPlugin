using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp;
using com.github.lhervier.ksp.ui.styles;

namespace com.github.lhervier.ksp.ui.ugui
{
    /// <summary>Custom title bar for the cheat sheet uGUI window (ksp_cheatsheet .ktb).</summary>
    internal static class CheatSheetUGUITitleBar
    {
        public const string ObjectName = "CheatSheetTitleBar";

        private static Sprite iconSprite;
        private static Sprite GamepadIconSprite
        {
            get
            {
                if (iconSprite != null)
                {
                    return iconSprite;
                }

                if (GameDatabase.Instance == null)
                {
                    return null;
                }

                var tex = GameDatabase.Instance.GetTexture(
                    SteamInputPalette.GamepadIconPath, 
                    false
                );
                if (tex == null)
                {
                    return null;
                }

                iconSprite = Sprite.Create(
                    tex,
                    new Rect(0f, 0f, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f),
                    100f
                );
                iconSprite.hideFlags = HideFlags.HideAndDontSave;
                return iconSprite;
            }
        }

        private static Sprite headerFillSprite;
        private static Sprite HeaderFillSprite
        {
            get
            {
                if (headerFillSprite != null)
                {
                    return headerFillSprite;
                }

                var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                tex.SetPixel(0, 0, Color.white);
                tex.Apply();
                tex.filterMode = FilterMode.Point;
                tex.hideFlags = HideFlags.HideAndDontSave;
                headerFillSprite = Sprite.Create(tex, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 100f);
                headerFillSprite.hideFlags = HideFlags.HideAndDontSave;
                return headerFillSprite;
            }
        }

        private const float PaddingLeft = 8f;
        private const float PaddingRight = 8f;
        private const float PaddingTop = 5f;
        private const float PaddingBottom = 5f;
        private const float BottomBorderHeight = 1f;
        private static readonly Color TitleBarBottomBorder = new Color(68f / 255f, 68f / 255f, 68f / 255f);

        public static void Build(PopupDialog dialog)
        {
            if (dialog == null || dialog.popupWindow == null)
            {
                return;
            }

            var window = dialog.popupWindow.transform;
            if (window.Find(ObjectName) != null)
            {
                return;
            }

            var titleBarGo = new GameObject(ObjectName, typeof(RectTransform));
            titleBarGo.transform.SetParent(window, false);
            // Draw above dialog content (same canvas: later sibling = in front).
            titleBarGo.transform.SetAsLastSibling();

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
            headerImage.sprite = HeaderFillSprite;
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
            bottomBorderImage.sprite = HeaderFillSprite;
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
            iconImage.sprite = GamepadIconSprite;
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

            ReserveContentSpaceBelowTitleBar(window);
            LayoutRebuilder.ForceRebuildLayoutImmediate(titleBarRect);
        }

        private static void ReserveContentSpaceBelowTitleBar(Transform window)
        {
            foreach (Transform child in window)
            {
                if (child.name == ObjectName || child.name == "Title")
                {
                    continue;
                }

                var rt = child as RectTransform;
                if (rt == null)
                {
                    continue;
                }

                rt.offsetMax = new Vector2(rt.offsetMax.x, -SteamInputPalette.TitleBarHeight);
            }
        }
    }
}
