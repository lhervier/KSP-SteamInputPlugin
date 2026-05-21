using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using com.github.lhervier.ksp.ui.styles;

namespace com.github.lhervier.ksp.ui.ugui
{
    /// <summary>
    /// uGUI test shell via KSP PopupDialog (same approach as Trajectories MainGUI).
    /// </summary>
    internal sealed class CheatSheetUGUIWindow
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("CheatSheetUGUIWindow");
        public const string TITLEBAR_OBJECT_NAME = "CheatSheetTitleBar";
        private const string DIALOG_ID = "SteamInputCheatSheetUGUI";
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
                _popupDialog = CreatePopupDialog();
                if( _popupDialog == null ) return;
                
                AddTitleBar(_popupDialog);
                CheatSheetUGUIChrome.Apply(_popupDialog);
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

        private PopupDialog CreatePopupDialog()
        {
            // Creates a ultra minimal MultiOptionDialog. We will not use it.
            var pos = NormalizedWindowPos(ScreenX, ScreenYFromTop, SteamInputPalette.WindowWidth, WindowHeight);
            var content = new DialogGUIVerticalLayout();
            MultiOptionDialog multiOptionDialog = new MultiOptionDialog(
                DIALOG_ID,
                string.Empty,
                string.Empty,
                HighLogic.UISkin,
                new Rect(pos.x, pos.y, SteamInputPalette.WindowWidth, WindowHeight),
                new DialogGUIBase[]
                {
                    new DialogGUIBox(null, -1, -1, () => true, content)
                }
            );
            
            // Creates the popup dialog
            PopupDialog popupDialog = PopupDialog.SpawnPopupDialog(
                multiOptionDialog,
                true,
                HighLogic.UISkin,
                false,
                string.Empty
            );
            if( popupDialog == null || popupDialog.popupWindow == null )
            {
                return null;
            }
            popupDialog.onDestroy.AddListener(new UnityAction(() => OnPopupDestroy()));
            return popupDialog;
        }

        private static void AddTitleBar(PopupDialog popupDialog)
        {
            // Check if title bar already exist
            if( popupDialog.popupWindow.transform.Find(TITLEBAR_OBJECT_NAME) != null ) {
                return;
            }

            // Create the title bar, and add it to the popup dialog
            GameObject titleBarGo = TitleBarBuilder.Create(TITLEBAR_OBJECT_NAME);
            titleBarGo.transform.SetParent(popupDialog.popupWindow.transform, false);
            titleBarGo.transform.SetAsLastSibling();

            // Décale tous les transform vers le bas
            foreach (Transform child in popupDialog.popupWindow.transform)
            {
                if( child == null ) continue;
                if (child.name == TITLEBAR_OBJECT_NAME) continue;
                if( child is RectTransform rt)
                {
                    rt.offsetMax = new Vector2(rt.offsetMax.x, -SteamInputPalette.TitleBarHeight);
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(titleBarGo.GetComponent<RectTransform>());
        }

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
