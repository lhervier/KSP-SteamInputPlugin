using System.Collections.Generic;
using com.github.lhervier.ksp.model;

namespace com.github.lhervier.ksp.ui.model
{
    public class UIPhysicalZone
    {
        public EGamepadZone Zone { get; set; }
        public string Label { get; set; }
        public string GroupId { get; set; }
        public List<string> ModeshiftGroupIds { get; set; } = new List<string>();
    }
}