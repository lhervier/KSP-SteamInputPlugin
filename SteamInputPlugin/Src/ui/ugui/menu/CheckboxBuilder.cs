using UnityEngine;
using UnityEngine.UI;
using System;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using com.github.lhervier.ksp.ugui.shared.styles;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.menu
{
    public class CheckboxBuilder
    {
        private CheatSheetViewModel _viewModel;

        public CheckboxBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public CheckboxController Create(
            bool initialChecked, 
            Action<bool> onToggle
        )
        {
            var go = new GameObject("Checkbox", typeof(RectTransform));
            CheckboxController controller = go.AddComponent<CheckboxController>();
            controller.Initialize(_viewModel);

            // Fixed 12x12 square
            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = DefaultPalette.CheckboxSize;
            layoutElement.preferredHeight = DefaultPalette.CheckboxSize;
            layoutElement.minWidth = DefaultPalette.CheckboxSize;
            layoutElement.minHeight = DefaultPalette.CheckboxSize;

            // Dark background
            var bgImage = go.AddComponent<Image>();
            bgImage.sprite = SpritesGlobal.FillSprite;
            bgImage.type = Image.Type.Simple;
            bgImage.color = DefaultPalette.FieldBackgroundColor;
            bgImage.raycastTarget = true;

            // Green inner fill that represents the "checked" state
            var checkmarkGo = new GameObject("Checkmark", typeof(RectTransform));
            checkmarkGo.transform.SetParent(go.transform, false);
            controller.BindCheckmark(checkmarkGo);

            var checkmarkRect = checkmarkGo.GetComponent<RectTransform>();
            checkmarkRect.anchorMin = Vector2.zero;
            checkmarkRect.anchorMax = Vector2.one;
            checkmarkRect.offsetMin = new Vector2(DefaultPalette.CheckmarkInset, DefaultPalette.CheckmarkInset);
            checkmarkRect.offsetMax = new Vector2(-DefaultPalette.CheckmarkInset, -DefaultPalette.CheckmarkInset);

            var checkmarkImage = checkmarkGo.AddComponent<Image>();
            checkmarkImage.sprite = SpritesGlobal.FillSprite;
            checkmarkImage.type = Image.Type.Simple;
            checkmarkImage.color = DefaultPalette.AccentColor;
            checkmarkImage.raycastTarget = false;

            // Initial visibility from the parameter; checkmarkGo.activeSelf is the source of truth
            // (so external updates via SetActive stay consistent with what the click handler reads).
            checkmarkGo.SetActive(initialChecked);

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
            // checkmarkGo.activeSelf is the current (pre-click) visual state, so the new desired
            // state is its negation. The callee decides whether to apply it (e.g., by reading
            // the model's state, calling a toggle method, etc.).
            button.onClick.AddListener(() => onToggle(!checkmarkGo.activeSelf));

            return controller;
        }

        public class CheckboxController : BaseSteamInputController
        {
            private GameObject _checkMark;

            public void BindCheckmark(GameObject checkmark)
            {
                _checkMark = checkmark;
            }

            public bool IsChecked()
            {
                if( _checkMark == null ) return false;
                return _checkMark.activeInHierarchy;
            }

            public void SetChecked(bool isChecked)
            {
                _checkMark.SetActive(isChecked);
            }
        }
    }
}
