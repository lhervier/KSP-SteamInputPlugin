using com.github.lhervier.ksp;

namespace com.github.lhervier.ksp.ui.model
{
    public class UIActivator
    {
        public EInput Input { get; set; }
        public bool LongPress { get; set; } = false;
        public string BindingText { get; set;}
        public bool ModeShift { get; set; }
    }
}