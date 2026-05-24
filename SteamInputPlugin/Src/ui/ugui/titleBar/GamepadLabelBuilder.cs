using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using UnityEngine.Events;

namespace com.github.lhervier.ksp.ui.ugui.titleBar
{
    public class GamepadLabelBuilder
    {
        private CheatSheetViewModel _viewModel;
        
        public GamepadLabelBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public GamepadLabelController Create()
        {
            var go = new GameObject("SteamInput.TitleBar.RightColumn.GamepadName", typeof(RectTransform));
            var controller = go.AddComponent<GamepadLabelController>();
            controller.Initialize(this._viewModel);
            
            var label = go.AddComponent<Text>();
            label.text = "<gamepad>";
            label.font = HighLogic.UISkin.font;
            label.fontSize = 10;
            label.color = SteamInputPalette.TitleBarControllerNameColor;
            label.alignment = TextAnchor.MiddleLeft;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;
            controller.BindLabel(label);
            
            return controller;
        }

        /// <summary>
        /// Pushes the gamepad label from the ViewModel into a Text component.
        /// Subscribes on Bind, unsubscribes on OnDestroy.
        /// </summary>
        public class GamepadLabelController : BaseSteamInputController
        {
            private Text _label;

            public void BindLabel(Text label)
            {
                this._label = label;
            }

            public void Start()
            {
                this.ViewModel.OnGamepadLabelChanged.Add(OnLabelChanged);
                OnLabelChanged(this.ViewModel.GamepadLabel);
            }

            public void OnDestroy()
            {
                this.ViewModel?.OnGamepadLabelChanged.Remove(OnLabelChanged);
            }

            private void OnLabelChanged(string value)
            {
                if (this._label != null)
                {
                    this._label.text = value;
                }
            }
        }
    }
}