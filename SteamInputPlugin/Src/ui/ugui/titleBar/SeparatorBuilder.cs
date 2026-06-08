using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using com.github.lhervier.ksp.ugui.shared.styles;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.titleBar
{
    public class SeparatorBuilder
    {
        private CheatSheetViewModel _viewModel;

        public SeparatorBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public SeparatorController Create()
        {
            var separatorGo = new GameObject("SteamInput.TitleBar.Separator", typeof(RectTransform));
            SeparatorController controller = separatorGo.AddComponent<SeparatorController>();
            controller.Initialize(this._viewModel);
            
            // Stretched horizontally, positionned at the bottom of the parent
            var separatorRect = separatorGo.GetComponent<RectTransform>();
            separatorRect.anchorMin = new Vector2(0f, 0f);
            separatorRect.anchorMax = new Vector2(1f, 0f);
            separatorRect.pivot = new Vector2(0.5f, 0f);
            separatorRect.sizeDelta = new Vector2(0f, PopupPalette.TitleBarSeparatorHeight);
            separatorRect.anchoredPosition = Vector2.zero;
            
            // The separator
            var separatorImage = separatorGo.AddComponent<Image>();
            separatorImage.sprite = SpritesGlobal.FillSprite;
            separatorImage.type = Image.Type.Simple;
            separatorImage.color = PopupPalette.TitleBarSeparatorColor;
            separatorImage.raycastTarget = false;

            return controller;
        }

        public class SeparatorController : BaseSteamInputController
        {
        }
    }
}