using com.github.lhervier.ksp.model;

namespace com.github.lhervier.ksp.ui.model
{
    public class UIActionGroupZone : ActionGroupZone
    {
        public string Label { get; set; }
        public bool First { get; set; }
        public bool Last { get; set; }
    }
}