using UnityEngine;

namespace com.github.lhervier.ksp.ui
{
    /// <summary>
    /// Simple icon loaded from GameDatabase (see VesselBookmarkIcon).
    /// </summary>
    public class SteamInputIcon
    {
        private readonly Texture2D _texture;
        private readonly int _width;
        private readonly int _height;

        public SteamInputIcon(string gameDatabasePath, int width, int height)
        {
            _width = width;
            _height = height;
            if (GameDatabase.Instance != null)
            {
                _texture = GameDatabase.Instance.GetTexture(gameDatabasePath, false);
            }
        }

        public void Draw()
        {
            if (_texture == null)
            {
                return;
            }

            Rect iconRect = GUILayoutUtility.GetRect(
                _width,
                _height,
                GUILayout.Width(_width),
                GUILayout.Height(_height)
            );
            GUI.DrawTexture(iconRect, _texture, ScaleMode.ScaleToFit);
        }
    }
}
