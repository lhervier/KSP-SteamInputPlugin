using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine.SceneManagement;

namespace com.github.lhervier.ksp 
{
    public enum ActionGroup {
        None,
        MenuControls,
        TrackingStationControls,
        EditorControls,
        FlightControls,
        MapFlightControls,
        DockingControls,
        MapDockingControls,
        EvaControls,
        MapEvaControls,
        EvaConstructionModeControls,
        FreeIvaControls,
        MissionBuilderControls
    }
}
