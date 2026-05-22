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

        public GameObject Create()
        {
            var go = new GameObject("SteamInput.TitleBar.RightColumn.GamepadName", typeof(RectTransform));

            var label = go.AddComponent<Text>();
            label.text = "<gamepad>";
            label.font = HighLogic.UISkin.font;
            label.fontSize = 10;
            label.color = SteamInputPalette.TitleBarControllerNameColor;
            label.alignment = TextAnchor.MiddleLeft;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;

            // Push the current action group label and react to changes via the ViewModel event
            var binder = go.AddComponent<GamepadLabelBinder>();
            binder.Bind(this._viewModel, label);
            
            return go;
        }

        /// <summary>
        /// Pushes the gamepad label from the ViewModel into a Text component.
        /// Subscribes on Bind, unsubscribes on OnDestroy.
        /// </summary>
        private class GamepadLabelBinder : MonoBehaviour
        {
            private CheatSheetViewModel _viewModel;
            private Text _label;

            public void Bind(CheatSheetViewModel viewModel, Text label)
            {
                this._viewModel = viewModel;
                this._label = label;

                this._viewModel.OnGamepadLabelChanged.Add(OnLabelChanged);
                OnLabelChanged(this._viewModel.GamepadLabel);
            }

            public void OnDestroy()
            {
                this._viewModel?.OnGamepadLabelChanged.Remove(OnLabelChanged);
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