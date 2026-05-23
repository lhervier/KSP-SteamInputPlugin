namespace com.github.lhervier.ksp.ui.model
{
    public class UIPhysicalZone : PhysicalZone
    {
        public string Label { get; set; }
        public bool Visible { get; set; }
        public bool First { get; set; }
        public bool Last { get; set; }
    }
}