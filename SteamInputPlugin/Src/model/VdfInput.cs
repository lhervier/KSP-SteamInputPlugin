using System.Collections.Generic;
using com.github.lhervier.ksp.ui.model;

namespace com.github.lhervier.ksp.model
{
    public class VdfInput
    {
        public EInput Name { get; set; }
        public List<VdfActivator> Activators { get; set; }
    }
}