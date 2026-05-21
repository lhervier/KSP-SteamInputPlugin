using com.github.lhervier.ksp.ui.styles;
using UnityEngine;
using UnityEngine.Events;

namespace com.github.lhervier.ksp.ui.ugui
{
    /// <summary>
    /// uGUI test shell via KSP PopupDialog (same approach as Trajectories MainGUI).
    /// </summary>
    internal sealed class CheatSheetUGUIWindow
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("CheatSheetUGUIWindow");
        public const string TITLEBAR_OBJECT_NAME = "CheatSheetTitleBar";
        public const string DIALOG_ID = "SteamInputCheatSheetUGUI";
        private const float WindowHeight = 320f;
        private const float ScreenX = 428f;
        private const float ScreenYFromTop = 20f;

        private PopupDialog _popupDialog = null;

        // ===============================================================
        // Public API
        // ===============================================================

        public void Show()
        {
            if (_popupDialog == null)
            {
                CreatePopup();
                if( _popupDialog == null ) return;
                
                AddTitleBar(_popupDialog);
            }

            _popupDialog.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _popupDialog?.gameObject.SetActive(false);
        }

        public void Destroy()
        {
            _popupDialog?.Dismiss();
            _popupDialog = null;
        }

        private void OnPopupDestroy()
        {
            _popupDialog = null;
        }

        // ===============================================================
        // Helpers
        // ===============================================================

        private void CreatePopup()
        {
            _popupDialog = PopupDialogBuilder.Create(
                ScreenX, 
                ScreenYFromTop, 
                SteamInputPalette.WindowWidth,
                WindowHeight,
                new UnityAction(() => OnPopupDestroy())
            );
        }

        private void AddTitleBar(PopupDialog popupDialog)
        {
            GameObject titleBarGo = TitleBarBuilder.Create(TITLEBAR_OBJECT_NAME);
            titleBarGo.transform.SetParent(popupDialog.popupWindow.transform, false);
        }
    }
}
