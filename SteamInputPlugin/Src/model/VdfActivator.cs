using System.Collections.Generic;
using com.github.lhervier.ksp.steaminput.ui.model;

namespace com.github.lhervier.ksp.steaminput.model
{
    public class VdfActivator
    {
        public EActivator Name { get; set; }
        public List<VdfBinding> Bindings { get; set; }
    }
}