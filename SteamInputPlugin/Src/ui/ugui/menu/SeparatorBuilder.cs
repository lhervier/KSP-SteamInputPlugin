using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using com.github.lhervier.ksp.uigui.shared.styles;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.menu
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
            var go = new GameObject("Separator", typeof(RectTransform));
            SeparatorController controller = go.AddComponent<SeparatorController>();
            controller.Initialize(_viewModel);

            // 1px tall, full width (the parent VLG stretches it via childForceExpandWidth = true)
            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 1f;
            layoutElement.minHeight = 1f;

            var image = go.AddComponent<Image>();
            image.sprite = SpritesGlobal.FillSprite;
            image.type = Image.Type.Simple;
            image.color = DefaultPalette.SeparatorColor;
            image.raycastTarget = false;

            return controller;
        }

        public class SeparatorController : BaseSteamInputController
        {
        }
    }
}