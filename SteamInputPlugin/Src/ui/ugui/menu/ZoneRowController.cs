using com.github.lhervier.ksp.steaminput.ui.model;
using com.github.lhervier.ksp.shared.ugui.checkbox;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.menu
{
    public class ZoneRowController : BaseSteamInputController
    {
        // =========================================
        // Life cycle
        // =========================================

        private UIConfigZone _zone;
        public ZoneRowController WithZone(UIConfigZone zone)
        {
            _zone = zone;
            return this;
        }

        private CheckboxController _checkboxController;
        public ZoneRowController WithCheckboxController(CheckboxController checkboxController)
        {
            _checkboxController = checkboxController;
            return this;
        }

        private ArrowsController _arrowsController;
        public ZoneRowController WithArrowsController(ArrowsController arrowsController)
        {
            _arrowsController = arrowsController;
            return this;
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

        // =========================================
        // Methods bound to events
        // =========================================

        private void OnToggled(bool isChecked)
        {
            ViewModel.SetZoneVisibility(_zone, isChecked);
        }

        // =========================================
        // Public API
        // =========================================

        public void UpdateZone(UIConfigZone zone)
        {
            this._arrowsController?.UpdateZone(zone);
            this._checkboxController?.SetChecked(zone.Visible);
            this._checkboxController?.SetLabel(zone.Label);
        }
    }
}
