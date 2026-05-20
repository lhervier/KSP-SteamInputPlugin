using System.Collections.Generic;
using com.github.lhervier.ksp;
using com.github.lhervier.ksp.ui.model;
using com.github.lhervier.ksp.ui.styles;
using UnityEngine;

namespace com.github.lhervier.ksp.ui
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
                    GUILayout.Box(GUIContent.none, SteamInputStyles.ZoneSeparator, GUILayout.ExpandWidth(true));
                }
            }
            GUILayout.EndVertical();
        }

        private static void DrawZone(UIPhysicalZone zone)
        {
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            GUILayout.BeginHorizontal(SteamInputStyles.ZoneHeaderBar, GUILayout.ExpandWidth(true));
            GUILayout.Label(zone.Label.ToUpperInvariant(), SteamInputStyles.ZoneName, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

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

        private static void DrawSection(string title, GUIStyle sectionStyle)
        {
            GUILayout.Label(title, sectionStyle, GUILayout.ExpandWidth(true));
        }
    }
}
