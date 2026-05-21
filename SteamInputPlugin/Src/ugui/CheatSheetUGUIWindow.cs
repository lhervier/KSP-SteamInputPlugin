using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.ui.styles;

namespace com.github.lhervier.ksp.ugui
{
    /// <summary>
    /// Minimal uGUI shell for comparing chrome with the IMGUI window (no title bar).
    /// </summary>
    internal sealed class CheatSheetUGUIWindow
    {
        private const int CanvasSortOrder = 30000;
        private const float BorderPixels = 1f;
        private const float PanelScreenX = 428f;
        private const float PanelScreenY = 20f;
        private const float PanelHeight = 320f;

        private GameObject root;
        private RectTransform panelRect;
        private static Sprite solidSprite;

        public void Show()
        {
            if (root == null || panelRect == null)
            {
                if (root != null)
                {
                    Object.Destroy(root);
                    root = null;
                    panelRect = null;
                }
                Build();
            }
            root.SetActive(true);
        }

        public void Hide()
        {
            if (root != null)
            {
                root.SetActive(false);
            }
        }

        public void Destroy()
        {
            if (root != null)
            {
                Object.Destroy(root);
                root = null;
                panelRect = null;
            }
        }

        private void Build()
        {
            root = new GameObject("CheatSheetUGUI");
            Object.DontDestroyOnLoad(root);

            var canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = CanvasSortOrder;
            canvas.pixelPerfect = true;

            root.AddComponent<GraphicRaycaster>();

            var panelGo = new GameObject("Panel", typeof(RectTransform));
            panelRect = panelGo.GetComponent<RectTransform>();
            panelRect.SetParent(root.transform, false);
            panelRect.anchorMin = new Vector2(0f, 1f);
            panelRect.anchorMax = new Vector2(0f, 1f);
            panelRect.pivot = new Vector2(0f, 1f);
            panelRect.anchoredPosition = new Vector2(PanelScreenX, -PanelScreenY);
            panelRect.sizeDelta = new Vector2(SteamInputPalette.WindowWidth, PanelHeight);

            var borderGo = CreateStretchChild(panelRect, "Border");
            var borderImage = borderGo.AddComponent<Image>();
            SetupSolidImage(borderImage, SteamInputPalette.Border);

            var backgroundGo = CreateStretchChild(panelRect, "Background");
            var backgroundRect = backgroundGo.GetComponent<RectTransform>();
            backgroundRect.offsetMin = new Vector2(BorderPixels, BorderPixels);
            backgroundRect.offsetMax = new Vector2(-BorderPixels, -BorderPixels);

            var backgroundImage = backgroundGo.AddComponent<Image>();
            SetupSolidImage(backgroundImage, SteamInputPalette.Body);
        }

        private static void SetupSolidImage(Image image, Color color)
        {
            image.sprite = SolidSprite;
            image.type = Image.Type.Simple;
            image.color = color;
            image.raycastTarget = false;
        }

        private static Sprite SolidSprite
        {
            get
            {
                if (solidSprite != null)
                {
                    return solidSprite;
                }

                var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                tex.SetPixel(0, 0, Color.white);
                tex.Apply();
                tex.hideFlags = HideFlags.HideAndDontSave;
                solidSprite = Sprite.Create(tex, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
                solidSprite.hideFlags = HideFlags.HideAndDontSave;
                return solidSprite;
            }
        }

        private static GameObject CreateStretchChild(RectTransform parentRect, string name)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parentRect, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return go;
        }
    }
}
