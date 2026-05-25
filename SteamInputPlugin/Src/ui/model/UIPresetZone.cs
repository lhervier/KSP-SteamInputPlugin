using com.github.lhervier.ksp.model;

namespace com.github.lhervier.ksp.ui.model
{
    public class UIPresetZone
    {
        public EGamepadZone Zone { get; set; }
        public string Label { get; set; }
        public string GroupId { get; set; }
        public string ModeshiftGroupId { get; set; }
        public bool First { get; set; }
        public bool Last { get; set; }
    }
}