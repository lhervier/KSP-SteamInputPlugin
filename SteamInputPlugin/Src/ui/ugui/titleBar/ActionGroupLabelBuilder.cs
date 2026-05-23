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

        public ActionGroupLabelController Create()
        {
            var badgeGo = new GameObject("SteamInput.TitleBar.RightColumn.ActionGroup", typeof(RectTransform));
            var controller = badgeGo.AddComponent<ActionGroupLabelController>();
            controller.Initialize(this._viewModel);
            
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
            label.color = SteamInputPalette.DefaultAccentColor;
            label.alignment = TextAnchor.MiddleCenter;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;
            controller.InitLabel(label);

            return controller;
        }

        /// <summary>
        /// Pushes the action group label from the ViewModel into a Text component.
        /// Subscribes on Bind, unsubscribes on OnDestroy.
        /// </summary>
        public class ActionGroupLabelController : BaseSteamInputController
        {
            private Text _label;

            public void InitLabel(Text label)
            {
                this._label = label;
            }

            public void Start()
            {
                this.ViewModel.OnActionGroupLabelChanged.Add(OnLabelChanged);
                OnLabelChanged(this.ViewModel.ActionGroupLabel);
            }

            public void OnDestroy()
            {
                this.ViewModel?.OnActionGroupLabelChanged.Remove(OnLabelChanged);
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