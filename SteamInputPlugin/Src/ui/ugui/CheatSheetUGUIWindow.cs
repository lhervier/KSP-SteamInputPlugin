using UnityEngine;
using UnityEngine.Events;
using com.github.lhervier.ksp.ui.styles;

namespace com.github.lhervier.ksp.ui.ugui
{
    /// <summary>
    /// uGUI test shell via KSP PopupDialog (same approach as Trajectories MainGUI).
    /// </summary>
    internal sealed class CheatSheetUGUIWindow
    {
        private const string DIALOG_ID = "SteamInputCheatSheetUGUI";
        private const float WindowWidth = SteamInputPalette.WindowWidth;
        private const float WindowHeight = 320f;
        private const float ScreenX = 428f;
        private const float ScreenYFromTop = 20f;

        private MultiOptionDialog _multiOptionDialog;
        private PopupDialog _popupDialog;

        // ===============================================================
        // Public API
        // ===============================================================

        public void Show()
        {
            if (_popupDialog == null)
            {
                var pos = NormalizedWindowPos(ScreenX, ScreenYFromTop, WindowWidth, WindowHeight);

                if (_multiOptionDialog != null)
                {
                    _multiOptionDialog.dialogRect.Set(pos.x, pos.y, WindowWidth, WindowHeight);
                    return;
                }

                var content = new DialogGUIVerticalLayout();

                _multiOptionDialog = new MultiOptionDialog(
                    DIALOG_ID,
                    string.Empty,
                    string.Empty,
                    HighLogic.UISkin,
                    new Rect(pos.x, pos.y, WindowWidth, WindowHeight),
                    new DialogGUIBase[]
                    {
                        new DialogGUIBox(null, -1, -1, () => true, content)
                    });
                    
                _popupDialog = PopupDialog.SpawnPopupDialog(
                    _multiOptionDialog,
                    true,
                    HighLogic.UISkin,
                    false,
                    string.Empty);
                CheatSheetUGUITitleBar.Build(_popupDialog);
                CheatSheetUGUIChrome.Apply(_popupDialog);
                _popupDialog.onDestroy.AddListener(new UnityAction(() => OnPopupDestroy()));
            }
            
            _popupDialog?.gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (_popupDialog != null)
            {
                _popupDialog.gameObject.SetActive(false);
            }
        }

        public void Destroy()
        {
            if (_popupDialog != null)
            {
                _popupDialog.Dismiss();
                _popupDialog = null;
            }
            _multiOptionDialog = null;
        }

        private void OnPopupDestroy()
        {
            _popupDialog = null;
        }

        // ===============================================================
        // Helpers
        // ===============================================================

        /// <summary>
        /// Normalized position from screen top-left, expressed as a percentage of the screen width and height.
        /// </summary>
        private static Vector2 NormalizedWindowPos(float screenX, float screenYFromTop, float width, float height)
        {
            var centerX = screenX + width * 0.5f;
            var centerY = Screen.height - screenYFromTop - height * 0.5f;
            return new Vector2(centerX / Screen.width, centerY / Screen.height);
        }

        
    }
}
