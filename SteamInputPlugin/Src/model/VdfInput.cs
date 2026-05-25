using System.Collections.Generic;

namespace com.github.lhervier.ksp.model
{
    public class VdfInput
    {
        public string Name { get; set; }
        public string Mode { get; set; }
        public List<VdfActivator> Activators { get; set; }
    }
}