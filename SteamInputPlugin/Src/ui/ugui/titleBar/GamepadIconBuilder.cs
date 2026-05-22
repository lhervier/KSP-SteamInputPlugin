using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using UnityEngine.Events;

namespace com.github.lhervier.ksp.ui.ugui.titleBar
{
    public class GamepadIconBuilder
    {
        private CheatSheetViewModel _viewModel;

        public GamepadIconBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public GameObject Create()
        {
            var iconGo = new GameObject("SteamInput.TitleBar.LeftColumn.Icon", typeof(RectTransform));

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
    }
}