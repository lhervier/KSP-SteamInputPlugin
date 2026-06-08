using com.github.lhervier.ksp.steaminput;

namespace com.github.lhervier.ksp.steaminput.ui.model
{
    /// <summary>
    /// One display-ready "row" of the cheat sheet (mockup .krow): an input icon, an
    /// optional short/long press chip, and an action label (highlighted with a note
    /// when the activator triggers a mode shift).
    /// </summary>
    public class UIActivator
    {
        public EInput Input { get; set; }
        public bool LongPress { get; set; } = false;
        public string BindingText { get; set;}
        public bool ModeShift { get; set; }

        /// <summary>Icon character for the input (mockup .kkbd), e.g. "↑", "A", "LB".</summary>
        public string IconText { get; set; }

        /// <summary>Press chip text (mockup .kpress), e.g. "long" / "short", or "" when not shown.</summary>
        public string PressText { get; set; } = "";

        /// <summary>Resolved action label (mockup .kaction): the binding label, or the mode shift word.</summary>
        public string ActionText { get; set; }

        /// <summary>True when the action should use the accent color (mockup .kaction.hi).</summary>
        public bool Highlighted { get; set; }

        /// <summary>Trailing note next to the action (mockup .knote), e.g. "(hold)", or "" when none.</summary>
        public string Note { get; set; } = "";
    }
}
