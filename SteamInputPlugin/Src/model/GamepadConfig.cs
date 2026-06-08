namespace com.github.lhervier.ksp.steaminput.model
{
    /// <summary>
    /// An exported controller configuration available in the Steam config folder.
    /// </summary>
    public class GamepadConfig
    {
        /// <summary>File name without extension, minus any trailing _&lt;index&gt; version suffix.</summary>
        public string Name { get; set; }

        /// <summary>The controller_mappings/title declared in the VDF.</summary>
        public string Title { get; set; }

        /// <summary>The controller the config targets (never null: unknown types are filtered out).</summary>
        public EControllerType ControllerType { get; set; }
    }
}
