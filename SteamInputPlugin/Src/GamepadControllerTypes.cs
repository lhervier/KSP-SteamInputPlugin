using System.Collections.Generic;

namespace com.github.lhervier.ksp
{
    /// <summary>
    /// Maps Steam Input <c>controller_type</c> values from VDF to display names (UI only, not localized).
    /// </summary>
    public static class GamepadControllerTypes
    {
        private static readonly Dictionary<string, string> DisplayNames = new Dictionary<string, string>
        {
            { "controller_steamcontroller_gordon", "Steam Controller" },
            { "controller_triton", "Steam Controller v2" },
            { "controller_hori_steam", "HORIPAD for Steam" },
            { "controller_ps4", "PlayStation 4" },
            { "controller_xboxelite", "Xbox Elite" },
        };

        public static string GetDisplayName(string controllerType)
        {
            if (string.IsNullOrEmpty(controllerType))
            {
                return "";
            }
            if (DisplayNames.TryGetValue(controllerType, out string displayName))
            {
                return displayName;
            }
            return controllerType;
        }
    }
}
