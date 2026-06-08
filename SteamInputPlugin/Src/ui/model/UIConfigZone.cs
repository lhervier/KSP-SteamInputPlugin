using com.github.lhervier.ksp.steaminput;

namespace com.github.lhervier.ksp.steaminput.ui.model
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