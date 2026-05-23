using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;

namespace com.github.lhervier.ksp.ui.ugui.menu
{
    public class CheckboxBuilder
    {
        private CheatSheetViewModel _viewModel;

        public CheckboxBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public GameObject Create()
        {
            var go = new GameObject("Checkbox", typeof(RectTransform));

            // Fixed 12x12 square
            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = SteamInputPalette.DefaultCheckboxSize;
            layoutElement.preferredHeight = SteamInputPalette.DefaultCheckboxSize;
            layoutElement.minWidth = SteamInputPalette.DefaultCheckboxSize;
            layoutElement.minHeight = SteamInputPalette.DefaultCheckboxSize;

            // Dark background
            var bgImage = go.AddComponent<Image>();
            bgImage.sprite = SpritesGlobal.FillSprite;
            bgImage.type = Image.Type.Simple;
            bgImage.color = SteamInputPalette.DefaultFieldBackgroundColor;
            bgImage.raycastTarget = true;

            // Green inner fill that represents the "checked" state
            var checkmarkGo = new GameObject("Checkmark", typeof(RectTransform));
            checkmarkGo.transform.SetParent(go.transform, false);
            var checkmarkRect = checkmarkGo.GetComponent<RectTransform>();
            checkmarkRect.anchorMin = Vector2.zero;
            checkmarkRect.anchorMax = Vector2.one;
            layoutElement.preferredHeight = SteamInputPalette.DefaultCheckboxSize;
            checkmarkRect.offsetMin = new Vector2(SteamInputPalette.DefaultCheckmarkInset, SteamInputPalette.DefaultCheckmarkInset);
            checkmarkRect.offsetMax = new Vector2(-SteamInputPalette.DefaultCheckmarkInset, -SteamInputPalette.DefaultCheckmarkInset);

            var checkmarkImage = checkmarkGo.AddComponent<Image>();
            checkmarkImage.sprite = SpritesGlobal.FillSprite;
            checkmarkImage.type = Image.Type.Simple;
            checkmarkImage.color = SteamInputPalette.DefaultAccentColor;
            checkmarkImage.raycastTarget = false;

            // Click toggles the checkmark visibility. State is captured by closure.
            bool isChecked = true;
            checkmarkGo.SetActive(isChecked);

            var button = go.AddComponent<Button>();
            button.targetGraphic = bgImage;
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.white;
            colors.pressedColor = Color.white;
            colors.selectedColor = Color.white;
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0f;
            button.colors = colors;
            button.onClick.AddListener(() => {
                isChecked = !isChecked;
                checkmarkGo.SetActive(isChecked);
                Debug.Log(isChecked ? "[SteamInput] Zone ON" : "[SteamInput] Zone OFF");
            });

            return go;
        }
    }
}
