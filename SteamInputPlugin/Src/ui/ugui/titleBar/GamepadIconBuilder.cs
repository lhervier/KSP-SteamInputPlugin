using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using com.github.lhervier.ksp.uigui.shared.styles;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.titleBar
{
    public class GamepadIconBuilder
    {
        private CheatSheetViewModel _viewModel;

        public GamepadIconBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public GamepadIconController Create()
        {
            var iconGo = new GameObject("SteamInput.TitleBar.LeftColumn.Icon", typeof(RectTransform));
            GamepadIconController controller = iconGo.AddComponent<GamepadIconController>();
            controller.Initialize(this._viewModel);

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
                    DefaultPalette.IconSize,
                    DefaultPalette.IconSize
                );
            }
            return controller;
        }

        public class GamepadIconController : BaseSteamInputController
        {
        }
    }
}