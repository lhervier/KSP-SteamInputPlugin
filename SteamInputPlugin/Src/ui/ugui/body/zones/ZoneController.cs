using com.github.lhervier.ksp.steaminput.ui.model;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.zones
{
    public class ZoneController : BaseSteamInputController
    {
        private ZoneHeaderController _zoneHeaderController;
        private ZoneBodyController _zoneBodyController;

        public ZoneController WithZoneHeaderController(ZoneHeaderController zoneHeaderController)
        {
            this._zoneHeaderController = zoneHeaderController;
            return this;
        }

        public ZoneController WithZoneBodyController(ZoneBodyController zoneBodyController)
        {
            _zoneBodyController = zoneBodyController;
            return this;
        }

        public void UpdateZone(UIPhysicalZone zone)
        {
            _zoneHeaderController?.UpdateZone(zone);
            _zoneBodyController?.UpdateZone(zone);
        }
    }
}
