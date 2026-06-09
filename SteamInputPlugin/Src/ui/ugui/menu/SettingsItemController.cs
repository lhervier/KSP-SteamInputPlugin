using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui.styles;
using com.github.lhervier.ksp.shared.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.menu
{
    public class SettingsItemController : BaseSteamInputController
    {
        // ==================================
        // Public API
        // ==================================

        public void OpenSettings()
        {
            ViewModel?.OpenSettings();
        }
    }
}
