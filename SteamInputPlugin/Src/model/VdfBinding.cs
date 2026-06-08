using System;

namespace com.github.lhervier.ksp.steaminput.model
{
    public class VdfBinding
    {
        public bool ModeShift { get; set; }
        
        // Non modeshift
        public string EventType { get; set; }
        public string Action { get; set; }
        public string Label { get; set; }
        
        // Modeshift binding
        public EGamepadZone Zone { get; set; }
        public string GroupId { get; set; }
    }
}