using System.Collections.Generic;

namespace com.github.lhervier.ksp.steaminput.model
{
    public class VdfGroup
    {
        public string GroupId { get; set; }
        public string Mode { get; set; }
        public List<VdfInput> Inputs { get; set; }
    }
}