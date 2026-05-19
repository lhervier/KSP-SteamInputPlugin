using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace com.github.lhervier.ksp
{
    /// <summary>
    /// Debug temporaire : log UnityEngine.Input à chaque frappe.
    /// Retirer ou désactiver après les tests AZERTY / Proton.
    /// </summary>
    public class UnityInputDebugLogger : MonoBehaviour
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("UnityInput");

        private static readonly KeyCode[] KeysOfInterest =
        {
            KeyCode.Slash,
            KeyCode.Period,
            KeyCode.Semicolon,
            KeyCode.Comma,
            KeyCode.M,
        };

        private void LateUpdate()
        {
            if (!Input.anyKeyDown && string.IsNullOrEmpty(Input.inputString))
                return;

            var sb = new StringBuilder();
            sb.Append("inputString='").Append(Input.inputString ?? "")
              .Append("' compositionString='").Append(Input.compositionString ?? "")
              .Append("'");

            var down = new List<string>();
            foreach (KeyCode k in Enum.GetValues(typeof(KeyCode)))
            {
                if (k == KeyCode.None)
                    continue;
                if (Input.GetKeyDown(k))
                    down.Add(k.ToString());
            }
            if (down.Count > 0)
                sb.Append(" | GetKeyDown: ").Append(string.Join(", ", down));

            sb.Append(" | interest: ");
            foreach (KeyCode k in KeysOfInterest)
            {
                if (Input.GetKeyDown(k))
                    sb.Append(k).Append("(DOWN) ");
                else if (Input.GetKey(k))
                    sb.Append(k).Append("(held) ");
            }

            LOGGER.LogInfo(sb.ToString());
        }
    }
}
