using System.Collections.Generic;
using com.github.lhervier.ksp;
using com.github.lhervier.ksp.ui.model;
using com.github.lhervier.ksp.ui.styles;
using UnityEngine;

namespace com.github.lhervier.ksp.ui.imgui
{
    /// <summary>
    /// Cheat sheet physical zones list (.kzone) — section headers only until inputs are wired.
    /// </summary>
    public class PhysicalZonesUI
    {
        private readonly CheatSheetViewModel viewModel;

        public PhysicalZonesUI(CheatSheetViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public void Draw()
        {
            List<UIPhysicalZone> zones = viewModel.GetPhysicalZones();
            if (zones == null || zones.Count == 0)
            {
                return;
            }

            GUILayout.BeginVertical(SteamInputStyles.ZoneListPanel, GUILayout.ExpandWidth(true));
            for (int i = 0; i < zones.Count; i++)
            {
                DrawZone(zones[i]);
                if (i < zones.Count - 1)
                {
                    GUILayout.Box(
                        GUIContent.none,
                        SteamInputStyles.ZoneSeparator,
                        GUILayout.ExpandWidth(true));
                }
            }
            GUILayout.EndVertical();
        }

        private static void DrawZone(UIPhysicalZone zone)
        {
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            DrawZoneHeader(zone.Label);

            GUILayout.BeginVertical(SteamInputStyles.ZoneBody, GUILayout.ExpandWidth(true));
            DrawSection(
                ModLocalization.GetString("SteamInput_sectionNormal").ToUpperInvariant(),
                SteamInputStyles.SectionNormal);
            DrawSection(
                "\u2193 " + ModLocalization.GetString("SteamInput_sectionModeshift").ToUpperInvariant(),
                SteamInputStyles.SectionModeshift);
            GUILayout.EndVertical();

            GUILayout.EndVertical();
        }

        private static void DrawZoneHeader(string label)
        {
            var content = new GUIContent(label.ToUpperInvariant());
            Rect headerRect = GUILayoutUtility.GetRect(
                content,
                SteamInputStyles.ZoneHeaderText,
                GUILayout.ExpandWidth(true),
                GUILayout.Height(SteamInputPalette.ZoneHeaderHeight));

            if (Event.current.type == EventType.Repaint)
            {
                GUI.DrawTexture(headerRect, PhysicalZoneStyles.ZoneHeaderBackground);
                GUI.DrawTexture(
                    new Rect(headerRect.xMin, headerRect.yMax - 1f, headerRect.width, 1f),
                    PhysicalZoneStyles.ZoneHeaderBottomLine);
            }

            GUI.Label(headerRect, content, SteamInputStyles.ZoneHeaderText);
        }

        private static void DrawSection(string title, GUIStyle sectionStyle)
        {
            GUILayout.Label(title, sectionStyle);
        }
    }
}
