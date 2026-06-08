using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using com.github.lhervier.ksp.steaminput.ui.ugui.body.zones;
using com.github.lhervier.ksp.steaminput.ui.ugui.body.selector;
using com.github.lhervier.ksp.steaminput.ui.ugui.body.settings;
using com.github.lhervier.ksp.ugui.shared;
using com.github.lhervier.ksp.uigui.shared.styles;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body
{
    /// <summary>
    /// Scrollable body of the popup (below the title bar). Content larger than the viewport
    /// produces a vertical scrollbar on the right.
    /// </summary>
    public class BodyBuilder : IUGUIBuilder<BodyController>
    {
        public const string BODY_NAME = "SteamInput.Body";
        
        private CheatSheetViewModel _viewModel;
        private SelectorBuilder _selectorBuilder;
        private ZoneListBuilder _zoneListBuilder;
        private SettingsBuilder _settingsBuilder;

        public BodyBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._selectorBuilder = new SelectorBuilder(viewModel);
            this._zoneListBuilder = new ZoneListBuilder(viewModel);
            this._settingsBuilder = new SettingsBuilder(viewModel);
        }

        public BodyController Create()
        {
            var bodyGo = new GameObject(BODY_NAME, typeof(RectTransform));
            var controller = bodyGo.AddComponent<BodyController>();
            controller.BindViewModel(_viewModel);
            controller.BindSelectorBuilder(_selectorBuilder);
            controller.BindZoneListBuilder(_zoneListBuilder);
            controller.BindSettingsBuilder(_settingsBuilder);

            // Escape KSP's VerticalLayoutGroup on popupWindow
            var layoutElement = bodyGo.AddComponent<LayoutElement>();
            layoutElement.ignoreLayout = true;

            // Fills the popup interior minus chrome (1px on left/right/bottom) and the title bar at the top
            var rect = bodyGo.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(SteamInputPalette.WindowBorderThickness, SteamInputPalette.WindowBorderThickness);
            rect.offsetMax = new Vector2(
                -SteamInputPalette.WindowBorderThickness,
                -(SteamInputPalette.WindowBorderThickness + SteamInputPalette.TitleBarHeight)
            );

            // ScrollRect drives the scrolling. It links viewport (clip) + content (scrolled) + scrollbar.
            var scrollRect = bodyGo.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 20f;

            // Viewport: full body minus the scrollbar column on the right. RectMask2D clips overflow.
            var viewportGo = new GameObject("Viewport", typeof(RectTransform));
            viewportGo.transform.SetParent(bodyGo.transform, false);

            var viewportRect = viewportGo.GetComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = new Vector2(-SteamInputPalette.MainScrollbarWidth, 0f);
            viewportGo.AddComponent<RectMask2D>();

            // Image with raycastTarget=true so mouse-wheel scroll has a target
            var viewportImage = viewportGo.AddComponent<Image>();
            viewportImage.sprite = SpritesGlobal.FillSprite;
            viewportImage.type = Image.Type.Simple;
            viewportImage.color = Color.clear;
            viewportImage.raycastTarget = true;
            scrollRect.viewport = viewportRect;

            // Content: child of the viewport, anchored to its top edge. Height is auto-managed
            // by the ContentSizeFitter below, based on the sum of children's preferred heights.
            var contentGo = new GameObject("Content", typeof(RectTransform));
            contentGo.transform.SetParent(viewportGo.transform, false);
            controller.BindContent(contentGo);

            var contentRect = contentGo.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = Vector2.zero;
            scrollRect.content = contentRect;

            // VLG stacks the children (the PhysicalZones list for now)
            var contentLayout = contentGo.AddComponent<VerticalLayoutGroup>();
            contentLayout.padding = new RectOffset(0, 0, 0, 0);
            contentLayout.spacing = 0f;
            contentLayout.childAlignment = TextAnchor.UpperLeft;
            contentLayout.childControlWidth = true;
            contentLayout.childControlHeight = true;
            contentLayout.childForceExpandWidth = true;
            contentLayout.childForceExpandHeight = false;

            // Auto-grow Content height to fit its children — drives the scrollbar's behavior
            var contentFitter = contentGo.AddComponent<ContentSizeFitter>();
            contentFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Scrollbar: vertical bar pinned to the right of the body, full height.
            var scrollbarGo = new GameObject("Scrollbar", typeof(RectTransform));
            scrollbarGo.transform.SetParent(bodyGo.transform, false);

            var scrollbarRect = scrollbarGo.GetComponent<RectTransform>();
            scrollbarRect.anchorMin = new Vector2(1f, 0f);
            scrollbarRect.anchorMax = new Vector2(1f, 1f);
            scrollbarRect.pivot = new Vector2(1f, 0.5f);
            scrollbarRect.sizeDelta = new Vector2(SteamInputPalette.MainScrollbarWidth, 0f);

            var scrollbarBg = scrollbarGo.AddComponent<Image>();
            scrollbarBg.sprite = SpritesGlobal.FillSprite;
            scrollbarBg.type = Image.Type.Simple;
            scrollbarBg.color = DefaultPalette.FieldBackgroundColor;
            scrollbarBg.raycastTarget = true;

            var scrollbar = scrollbarGo.AddComponent<Scrollbar>();
            scrollbar.direction = Scrollbar.Direction.BottomToTop;

            // Sliding area: where the handle slides. Anchored to fill the scrollbar.
            var slidingAreaGo = new GameObject("Sliding Area", typeof(RectTransform));
            slidingAreaGo.transform.SetParent(scrollbarGo.transform, false);

            var slidingAreaRect = slidingAreaGo.GetComponent<RectTransform>();
            slidingAreaRect.anchorMin = Vector2.zero;
            slidingAreaRect.anchorMax = Vector2.one;
            slidingAreaRect.offsetMin = Vector2.zero;
            slidingAreaRect.offsetMax = Vector2.zero;

            // Handle: the draggable part.
            var handleGo = new GameObject("Handle", typeof(RectTransform));
            handleGo.transform.SetParent(slidingAreaGo.transform, false);
            
            var handleRect = handleGo.GetComponent<RectTransform>();
            handleRect.anchorMin = Vector2.zero;
            handleRect.anchorMax = Vector2.one;
            handleRect.offsetMin = Vector2.zero;
            handleRect.offsetMax = Vector2.zero;

            var handleImage = handleGo.AddComponent<Image>();
            handleImage.sprite = SpritesGlobal.FillSprite;
            handleImage.type = Image.Type.Simple;
            // White so the Scrollbar's ColorBlock controls the tint without multiplication
            handleImage.color = Color.white;
            handleImage.raycastTarget = true;

            scrollbar.targetGraphic = handleImage;
            scrollbar.handleRect = handleRect;

            // Visible handle on dark scrollbar background, with subtle hover/press feedback
            var scrollbarColors = scrollbar.colors;
            scrollbarColors.normalColor = SteamInputPalette.WindowBorderColor;
            scrollbarColors.highlightedColor = SteamInputPalette.MainScrollbarColor;
            scrollbarColors.pressedColor = SteamInputPalette.MainScrollbarColor;
            scrollbarColors.selectedColor = SteamInputPalette.WindowBorderColor;
            scrollbarColors.disabledColor = SteamInputPalette.WindowBorderColor;
            scrollbarColors.colorMultiplier = 1f;
            scrollbarColors.fadeDuration = 0.1f;
            scrollbar.colors = scrollbarColors;

            scrollRect.verticalScrollbar = scrollbar;

            // The config picker, then the zones (VLG stacks them top to bottom).
            
            return controller;
        }
    }
}
