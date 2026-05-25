using com.github.lhervier.ksp;

namespace com.github.lhervier.ksp.ui.model
{
    public class UIConfigZone
    {
        public EGamepadZone Zone { get; set; }
        public string Label { get; set; }
        public bool Visible { get; set; }
        public bool First { get; set; }
        public bool Last { get; set; }
    }
}