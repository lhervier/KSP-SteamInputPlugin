namespace com.github.lhervier.ksp.ui.model
{
    /// <summary>
    /// A controller configuration as shown in the config picker: a display-ready title and
    /// controller label, plus the config name used to select / persist it.
    /// </summary>
    public class UIGamepadConfig
    {
        /// <summary>Config name, passed back to select the configuration.</summary>
        public string Name { get; set; }

        /// <summary>Title declared in the VDF (display).</summary>
        public string Title { get; set; }

        /// <summary>Localized controller type label (display).</summary>
        public string ControllerLabel { get; set; }
    }
}
