using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.shared.ugui.styles;
using com.github.lhervier.ksp.shared.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.menu
{
    public class SeparatorBuilder : IUGUIBuilder<SeparatorBuilder.SeparatorController>
    {
        public SeparatorController Build()
        {
            var go = new GameObject("Separator", typeof(RectTransform));
            
            // 1px tall, full width (the parent VLG stretches it via childForceExpandWidth = true)
            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 1f;
            layoutElement.minHeight = 1f;

            var image = go.AddComponent<Image>();
            image.sprite = SpritesGlobal.FillSprite;
            image.type = Image.Type.Simple;
            image.color = DefaultPalette.SeparatorColor;
            image.raycastTarget = false;

            return go.AddComponent<SeparatorController>();
        }

        public class SeparatorController : MonoBehaviour
        {
        }
    }
}