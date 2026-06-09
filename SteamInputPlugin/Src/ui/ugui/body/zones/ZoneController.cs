using com.github.lhervier.ksp.steaminput.ui.model;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.zones
{
    public class ZoneController : BaseSteamInputController
    {
        private ZoneHeaderController _zoneHeaderController;
        private ZoneBodyController _zoneBodyController;

        public void BindZoneHeaderController(ZoneHeaderController zoneHeaderController)
        {
            this._zoneHeaderController = zoneHeaderController;
        }

        public void BindZoneBodyController(ZoneBodyController zoneBodyController)
        {
            _zoneBodyController = zoneBodyController;
        }

        public void UpdateZone(UIPhysicalZone zone)
        {
            _zoneHeaderController?.UpdateZone(zone);
            _zoneBodyController?.UpdateZone(zone);
        }
    }
}
