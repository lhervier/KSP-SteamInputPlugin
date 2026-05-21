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
        private const string DialogId = "SteamInputCheatSheetUGUI";
        private const float WindowWidth = SteamInputPalette.WindowWidth;
        private const float WindowHeight = 320f;
        private const float ScreenX = 428f;
        private const float ScreenYFromTop = 20f;

        private MultiOptionDialog multiDialog;
        private PopupDialog popupDialog;

        public void Show()
        {
            if (popupDialog == null)
            {
                SpawnDialog();
            }
            if (popupDialog != null)
            {
                popupDialog.gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            if (popupDialog != null)
            {
                popupDialog.gameObject.SetActive(false);
            }
        }

        public void Destroy()
        {
            if (popupDialog != null)
            {
                popupDialog.Dismiss();
                popupDialog = null;
            }
            multiDialog = null;
        }

        private void SpawnDialog()
        {
            EnsureMultiDialog();

            popupDialog = PopupDialog.SpawnPopupDialog(
                multiDialog,
                false,
                HighLogic.UISkin,
                false,
                string.Empty);
            CheatSheetUGUIChrome.Apply(popupDialog);
            popupDialog.onDestroy.AddListener(new UnityAction(() => OnPopupDestroy()));
        }

        private void EnsureMultiDialog()
        {
            var pos = NormalizedWindowPos(ScreenX, ScreenYFromTop, WindowWidth, WindowHeight);

            if (multiDialog != null)
            {
                multiDialog.dialogRect.Set(pos.x, pos.y, WindowWidth, WindowHeight);
                return;
            }

            var content = new DialogGUIVerticalLayout();

            multiDialog = new MultiOptionDialog(
                DialogId,
                string.Empty,
                string.Empty,
                HighLogic.UISkin,
                new Rect(pos.x, pos.y, WindowWidth, WindowHeight),
                new DialogGUIBase[]
                {
                    new DialogGUIBox(null, -1, -1, () => true, content)
                });
        }

        /// <summary>
        /// Normalized position from screen top-left (see Trajectories MainGUI dialogRect).
        /// </summary>
        private static Vector2 NormalizedWindowPos(float screenX, float screenYFromTop, float width, float height)
        {
            var centerX = screenX + width * 0.5f;
            var centerY = Screen.height - screenYFromTop - height * 0.5f;
            return new Vector2(centerX / Screen.width, centerY / Screen.height);
        }

        private void OnPopupDestroy()
        {
            popupDialog = null;
        }
    }
}
