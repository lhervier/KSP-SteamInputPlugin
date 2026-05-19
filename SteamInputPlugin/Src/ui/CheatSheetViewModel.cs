namespace com.github.lhervier.ksp.ui
{
    public class CheatSheetViewModel
    {
        public string ControllerVdfPathBuffer { get; private set; }
        public CheatSheetViewModel() {
            ControllerVdfPathBuffer = SteamInputGlobalSettings.GetControllerVdfPath();
        }

        public string GetActionSetTitle()
        {
            string actionSetName = ActionGroupDaemon.Instance.GetCurrentActionGroup().ToString();
            if (string.IsNullOrEmpty(actionSetName))
            {
                return "—";
            }
            return SteamInputControllerVdf.GetActionSetTitle(actionSetName);
        }
    }
}