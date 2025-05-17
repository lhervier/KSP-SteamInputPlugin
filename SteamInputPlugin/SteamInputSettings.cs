using KSP.Localization;

namespace com.github.lhervier.ksp
{
    public class SteamInputSettings : GameParameters.CustomParameterNode
    {
        [GameParameters.CustomParameterUI("Show Logging Icon", toolTip = "Show the logging icon in the action bar", autoPersistance = true)]
        public bool showLoggingIcon = false;

        public override string Title => "Steam Input";
        public override string Section => "Steam Input";
        public override string DisplaySection => "Steam Input";
        public override int SectionOrder => 1;
        public override bool HasPresets => false;
        public override GameParameters.GameMode GameMode => GameParameters.GameMode.ANY;

        public static SteamInputSettings Instance => HighLogic.CurrentGame?.Parameters?.CustomParams<SteamInputSettings>();
    }
} 