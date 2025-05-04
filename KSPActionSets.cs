namespace com.github.lhervier.ksp {

    public enum KSPActionSets {
        Menu,
        MissionBuilder,
        TrackingStation,
        Editor,
        EVA,
        // ConstructionEVA,
        // FreeIVA,
        Flight,
        Docking,
        // FlightMap,
        // DockingMap
        Map
    }

    public static class KSPActionSetsUtils {
        public static string GetLabel(this KSPActionSets kac) {
            return kac.ToString() + " Controls";
        }

        public static string GetId(this KSPActionSets kac) {
            return kac.ToString() + "Controls";
        }
    }
}