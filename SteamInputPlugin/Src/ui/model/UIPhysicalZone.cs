using System.Collections.Generic;
using com.github.lhervier.ksp.steaminput.model;

namespace com.github.lhervier.ksp.steaminput.ui.model
{
    public class UIPhysicalZone
    {
        public EGamepadZone Zone { get; set; }
        public string Label { get; set; }
        public List<UISection> Sections { get; set; } = new List<UISection>();
    }
}
