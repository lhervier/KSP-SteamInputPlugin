using UnityEngine;
using KSP.UI.Screens;
using System;

namespace com.github.lhervier.ksp.ui
{
    public class TitleUI
    {
        private Action onClose;
        private CheatSheetViewModel viewModel;

        public TitleUI(CheatSheetViewModel viewModel, Action onClose)
        {
            this.viewModel = viewModel;
            this.onClose = onClose;
        }

        public void DrawTitle()
        {
            GUILayout.BeginHorizontal(SteamInputStyles.HeaderBar, GUILayout.Height(SteamInputStyles.TitleBarHeight));
            GUILayout.Label("AIDE MANETTE", SteamInputStyles.Title, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("×", SteamInputStyles.CloseButton))
            {
                onClose();
            }
            GUILayout.EndHorizontal();
        }
    }
}