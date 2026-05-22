using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using UnityEngine.Events;

namespace com.github.lhervier.ksp.ui.ugui.titleBar
{
    public class ActionGroupLabelBuilder
    {
        private CheatSheetViewModel _viewModel;
        
        public ActionGroupLabelBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public GameObject Create()
        {
            var badgeGo = new GameObject("SteamInput.TitleBar.RightColumn.ActionGroup", typeof(RectTransform));

            // Sliced sprite: transparent fill with a green border
            var image = badgeGo.AddComponent<Image>();
            image.sprite = SpritesTitleBar.ActionGroupBorderSprite;
            image.type = Image.Type.Sliced;
            image.color = Color.white;
            image.raycastTarget = false;

            // Padding around the text; badge size driven by content + padding via the HLG's reported preferredSize
            var layout = badgeGo.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(5, 5, 2, 2);
            layout.spacing = 0f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            // Green label
            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(badgeGo.transform, false);

            var label = labelGo.AddComponent<Text>();
            label.font = HighLogic.UISkin.font;
            label.fontSize = 10;
            label.color = SteamInputPalette.TitleBarActionGroupLabelColor;
            label.alignment = TextAnchor.MiddleCenter;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;

            // Push the current action group label and react to changes via the ViewModel event
            var binder = labelGo.AddComponent<ActionGroupLabelBinder>();
            binder.Bind(this._viewModel, label);

            return badgeGo;
        }

        /// <summary>
        /// Pushes the action group label from the ViewModel into a Text component.
        /// Subscribes on Bind, unsubscribes on OnDestroy.
        /// </summary>
        private class ActionGroupLabelBinder : MonoBehaviour
        {
            private CheatSheetViewModel _viewModel;
            private Text _label;

            public void Bind(CheatSheetViewModel viewModel, Text label)
            {
                this._viewModel = viewModel;
                this._label = label;

                this._viewModel.OnActionGroupLabelChanged.Add(OnLabelChanged);
                OnLabelChanged(this._viewModel.ActionGroupLabel);
            }

            public void OnDestroy()
            {
                this._viewModel?.OnActionGroupLabelChanged.Remove(OnLabelChanged);
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