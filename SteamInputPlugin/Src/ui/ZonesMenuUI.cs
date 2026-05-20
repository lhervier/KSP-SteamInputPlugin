using UnityEngine;
using com.github.lhervier.ksp;
using com.github.lhervier.ksp.ui.styles;

namespace com.github.lhervier.ksp.ui
{
    /// <summary>
    /// Title-bar "⋯" menu for zone visibility and order (.kmenu in ksp_cheatsheet mockup).
    /// </summary>
    public class ZonesMenuUI
    {
        private const float WindowChromePadding = 1f;

        private bool _open;
        private Rect _toggleButtonRect;
        private Rect _menuRect;

        public void DrawTitleBarButton()
        {
            if (GUILayout.Button("\u22EF", SteamInputStyles.TitleBarMenuButton))
            {
                _open = !_open;
            }

            if (Event.current.type == EventType.Repaint)
            {
                _toggleButtonRect = GUILayoutUtility.GetLastRect();
            }
        }

        public void HandleOutsideClick(Rect windowScreenRect)
        {
            if (!_open)
            {
                return;
            }

            UpdateMenuRect(windowScreenRect.width);
            HandleClickOutside(windowScreenRect);
        }

        public void DrawOverlay(float windowWidth)
        {
            if (!_open)
            {
                return;
            }

            UpdateMenuRect(windowWidth);

            if (Event.current.type == EventType.Repaint)
            {
                GUI.Box(_menuRect, GUIContent.none, SteamInputStyles.ZonesMenuPanel);
            }

            Rect contentRect = ZonesMenuStyles.ContentRect(_menuRect);
            GUILayout.BeginArea(contentRect);
            GUILayout.BeginVertical(GUILayout.Width(contentRect.width));

            DrawTitleHeader();

            GUILayout.Space(ZonesMenuStyles.SeparatorMargin);
            GUILayout.Box(
                GUIContent.none,
                SteamInputStyles.ZonesMenuSeparator,
                GUILayout.ExpandWidth(true),
                GUILayout.Height(ZonesMenuStyles.SeparatorHeight));
            GUILayout.Space(ZonesMenuStyles.SeparatorMargin);

            GUILayout.Space(ZonesMenuStyles.ContentPlaceholderHeight);

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        public void Close()
        {
            _open = false;
        }

        public bool IsOpen
        {
            get { return _open; }
        }

        private void UpdateMenuRect(float windowWidth)
        {
            float menuRight = GetMenuRightEdge(windowWidth);
            _menuRect = new Rect(
                menuRight - ZonesMenuStyles.PanelWidth,
                ZonesMenuStyles.PanelTop,
                ZonesMenuStyles.PanelWidth,
                CalcPanelHeight());
        }

        private float GetMenuRightEdge(float windowWidth)
        {
            if (_toggleButtonRect.width > 0f)
            {
                return _toggleButtonRect.xMax;
            }

            return windowWidth - ZonesMenuStyles.PanelRightInset;
        }

        private static void DrawTitleHeader()
        {
            // Match TitleUI: 28px header with 20px content band vertically centered (HeaderBar padding 4+4).
            GUILayout.BeginVertical(GUILayout.Height(ZonesMenuStyles.TitleHeaderHeight));
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical(GUILayout.Height(ZonesMenuStyles.TitleContentHeight));
            GUILayout.FlexibleSpace();
            GUILayout.Label(
                ModLocalization.GetString("SteamInput_zonesMenuTitle").ToUpperInvariant(),
                SteamInputStyles.ZonesMenuTitle);
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        private static float CalcPanelHeight()
        {
            return ZonesMenuStyles.TitleHeaderHeight
                + ZonesMenuStyles.SeparatorBlockHeight
                + ZonesMenuStyles.ContentPlaceholderHeight
                + ZonesMenuStyles.PanelBottomPadding;
        }

        private void HandleClickOutside(Rect windowScreenRect)
        {
            if (Event.current.type != EventType.MouseDown)
            {
                return;
            }

            Vector2 localMouse = ToWindowLocalMouse(windowScreenRect);
            if (_menuRect.Contains(localMouse) || _toggleButtonRect.Contains(localMouse))
            {
                return;
            }

            _open = false;
            Event.current.Use();
        }

        private static Vector2 ToWindowLocalMouse(Rect windowScreenRect)
        {
            Vector2 mouse = Event.current.mousePosition;
            mouse.x -= windowScreenRect.x + WindowChromePadding;
            mouse.y -= windowScreenRect.y + WindowChromePadding;
            return mouse;
        }
    }
}
