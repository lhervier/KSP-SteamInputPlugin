using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using UnityEngine.Events;
using System;

namespace com.github.lhervier.ksp.ui.ugui.menu
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
            var go = new GameObject("Separator", typeof(RectTransform));

            // 1px tall, full width (the parent VLG stretches it via childForceExpandWidth = true)
            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 1f;
            layoutElement.minHeight = 1f;

            var image = go.AddComponent<Image>();
            image.sprite = SpritesGlobal.FillSprite;
            image.type = Image.Type.Simple;
            image.color = SteamInputPalette.MenuSeparatorColor;
            image.raycastTarget = false;

            return go;
        }
    }
}