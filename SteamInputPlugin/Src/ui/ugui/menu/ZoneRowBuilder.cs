using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.steaminput.ui.model;
using com.github.lhervier.ksp.shared.ugui.styles;
using com.github.lhervier.ksp.shared.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui.checkbox;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.menu
{
    public class ZoneRowBuilder
    {

        private CheatSheetViewModel _viewModel;
        private CheckboxBuilder _checkBoxBuilder;
        private ArrowsBuilder _arrowsBuilder;

        public ZoneRowBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._checkBoxBuilder = new CheckboxBuilder();
            this._arrowsBuilder = new ArrowsBuilder(viewModel);
        }

        public ZoneRowController Create(UIConfigZone zone)
        {
            var rowGo = new GameObject("Zone." + zone.Zone.Name, typeof(RectTransform));
            var controller = rowGo.AddComponent<ZoneRowController>();
            controller.ViewModel(_viewModel);
            controller.BindZone(zone);

            // Background image: transparent normally, FieldBackground (#2a2a2a) on hover.
            // raycastTarget = true so pointer events fire on the row (including its empty area).
            var bgImage = rowGo.AddComponent<Image>();
            bgImage.sprite = SpritesGlobal.FillSprite;
            bgImage.type = Image.Type.Simple;
            bgImage.color = Color.clear;
            bgImage.raycastTarget = true;

            // PointerEnter/Exit on the row itself. Hovering a child (checkbox/arrows) does NOT
            // unhighlight the row, because in uGUI enter/exit propagate to the ancestors too.
            // The click-to-toggle is handled by the (greedy) checkbox below, not here.
            var trigger = rowGo.AddComponent<EventTrigger>();

            var enterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enterEntry.callback.AddListener(_ => bgImage.color = DefaultPalette.FieldBackgroundColor);
            trigger.triggers.Add(enterEntry);

            var exitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            exitEntry.callback.AddListener(_ => bgImage.color = Color.clear);
            trigger.triggers.Add(exitEntry);

            // Horizontal: checkbox (greedy: box + label fill the row) + arrows pushed to the right
            var layout = rowGo.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = DefaultPalette.Spacing;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            // Greedy checkbox: owns the label and the click-to-toggle over the whole row width.
            var checkboxController = this._checkBoxBuilder
                .Checked(zone.Visible)
                .Label(zone.Label)
                .Greedy(true)
                .Build();
            checkboxController.transform.SetParent(rowGo.transform, false);
            controller.BindCheckboxController(checkboxController);

            ArrowsBuilder.ArrowsController arrowsController = this._arrowsBuilder.Create(zone);
            arrowsController.transform.SetParent(rowGo.transform, false);
            controller.BindArrowsController(arrowsController);

            return controller;
        }

        public class ZoneRowController : BaseSteamInputController
        {
            private UIConfigZone _zone;
            private ArrowsBuilder.ArrowsController _arrowsController;
            private CheckboxController _checkboxController;

            public void BindZone(UIConfigZone zone)
            {
                _zone = zone;
            }

            public void BindCheckboxController(CheckboxController checkboxController)
            {
                _checkboxController = checkboxController;
            }

            public void BindArrowsController(ArrowsBuilder.ArrowsController arrowsController)
            {
                _arrowsController = arrowsController;
            }

            public void Start()
            {
                if( this._checkboxController != null )
                {
                    this._checkboxController.OnToggled.Add(OnToggled);
                }
            }

            public void OnDestroy()
            {
                if( this._checkboxController != null )
                {
                    this._checkboxController.OnToggled.Remove(OnToggled);
                }
            }

            private void OnToggled(bool isChecked)
            {
                ViewModel.SetZoneVisibility(_zone, isChecked);
            }

            public void UpdateZone(UIConfigZone zone)
            {
                this._arrowsController?.UpdateZone(zone);
                this._checkboxController?.SetChecked(zone.Visible);
                this._checkboxController?.SetLabel(zone.Label);
            }
        }
    }
}
