using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using UnityEngine.Events;

namespace com.github.lhervier.ksp.ui.ugui.titleBar
{
    public class SeparatorBuilder
    {
        private CheatSheetViewModel _viewModel;

        public SeparatorBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public GameObject Create()
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
    }
}