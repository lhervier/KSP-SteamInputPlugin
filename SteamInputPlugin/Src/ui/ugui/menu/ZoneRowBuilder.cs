using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.steaminput.ui.model;
using com.github.lhervier.ksp.shared.ugui.styles;
using com.github.lhervier.ksp.shared.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui.checkbox;
using com.github.lhervier.ksp.shared.ugui;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.menu
{
    public class ZoneRowBuilder : IUGUIBuilder<ZoneRowController>
    {
        // ==============================================
        // Builder parameters
        // ==============================================
        private CheatSheetViewModel _viewModel;
        public ZoneRowBuilder ViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        private UIConfigZone _zone;
        public ZoneRowBuilder Zone(UIConfigZone zone)
        {
            this._zone = zone;
            return this;
        }

        // ========================================
        // Build
        // ========================================

        public ZoneRowController Build()
        {
            var rowGo = new GameObject("Zone." + _zone.Zone.Name, typeof(RectTransform));
            
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
            var checkboxController = new CheckboxBuilder()
                .Checked(_zone.Visible)
                .Label(_zone.Label)
                .Greedy(true)
                .Build();
            checkboxController.transform.SetParent(rowGo.transform, false);
            
            ArrowsController arrowsController = new ArrowsBuilder()
                .ViewModel(_viewModel)
                .Zone(_zone)
                .Build();
            arrowsController.transform.SetParent(rowGo.transform, false);
            
            return rowGo
                .AddComponent<ZoneRowController>()
                .ViewModel(_viewModel)
                .Zone(_zone)
                .CheckboxController(checkboxController)
                .ArrowsController(arrowsController);
        }
    }
}
