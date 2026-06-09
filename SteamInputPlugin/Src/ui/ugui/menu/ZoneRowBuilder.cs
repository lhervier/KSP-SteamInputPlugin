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
        private ZoneLabelBuilder _zoneLabelBuilder;
        private ArrowsBuilder _arrowsBuilder;

        public ZoneRowBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._checkBoxBuilder = new CheckboxBuilder();
            this._zoneLabelBuilder = new ZoneLabelBuilder(viewModel);
            this._arrowsBuilder = new ArrowsBuilder(viewModel);
        }

        public ZoneRowController Create(UIConfigZone zone)
        {
            var rowGo = new GameObject("Zone." + zone.Zone.Name, typeof(RectTransform));
            var controller = rowGo.AddComponent<ZoneRowController>();
            controller.Initialize(_viewModel);
            controller.BindZone(zone);

            // Background image: transparent normally, FieldBackground (#2a2a2a) on hover.
            // raycastTarget = true so pointer events fire on the row (including its empty area).
            var bgImage = rowGo.AddComponent<Image>();
            bgImage.sprite = SpritesGlobal.FillSprite;
            bgImage.type = Image.Type.Simple;
            bgImage.color = Color.clear;
            bgImage.raycastTarget = true;

            // PointerEnter/Exit on the row itself. Hovering a child (button/checkbox) does NOT
            // unhighlight the row, because in uGUI a child is part of its parent's pointer chain.
            var trigger = rowGo.AddComponent<EventTrigger>();

            var enterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enterEntry.callback.AddListener(_ => bgImage.color = DefaultPalette.FieldBackgroundColor);
            trigger.triggers.Add(enterEntry);
            
            var exitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            exitEntry.callback.AddListener(_ => bgImage.color = Color.clear);
            trigger.triggers.Add(exitEntry);
            
            // Click on the row (anywhere not handled by a child) toggles the checkbox.
            // Clicks on the checkbox/arrows are consumed by their own Button components
            // (IPointerClickHandler stops at the first handler in the chain) so they don't reach here.
            var clickEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
            clickEntry.callback.AddListener(_ => this._viewModel.ToggleZoneVisibility(zone));
            trigger.triggers.Add(clickEntry);

            // Horizontal: checkbox + label (greedy) + arrows
            var layout = rowGo.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = DefaultPalette.Spacing;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            // Controller used to propagate changes from the viewModel into the GameObjects
            
            var checkboxController = this._checkBoxBuilder
                .Checked(zone.Visible)
                .Build();
            checkboxController.transform.SetParent(rowGo.transform, false);
            controller.BindCheckboxController(checkboxController);

            var labelController = this._zoneLabelBuilder.Create(zone.Label);
            labelController.transform.SetParent(rowGo.transform, false);
            controller.BindZoneLabelController(labelController);

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
            private ZoneLabelBuilder.ZoneLabelController _zoneLabelController;

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

            public void BindZoneLabelController(ZoneLabelBuilder.ZoneLabelController zoneLabelController)
            {
                this._zoneLabelController = zoneLabelController;
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
                this._zoneLabelController?.SetLabel(zone.Label);
            }
        }
    }
}
