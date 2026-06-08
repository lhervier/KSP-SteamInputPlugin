using System.Collections.Generic;
using com.github.lhervier.ksp.steaminput.ui.model;

namespace com.github.lhervier.ksp.steaminput.model
{
    public class VdfInput
    {
        public EInput Name { get; set; }
        public List<VdfActivator> Activators { get; set; }
    }
}