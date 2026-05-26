using System.Collections.Generic;
using com.github.lhervier.ksp.ui.model;

namespace com.github.lhervier.ksp.model
{
    public class VdfActivator
    {
        public EActivator Name { get; set; }
        public List<VdfBinding> Bindings { get; set; }
    }
}