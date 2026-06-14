using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using com.github.lhervier.ksp.steaminput.ui.model;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.shared;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared.ugui.styles;
using com.github.lhervier.ksp.shared.ugui.combo.itemcontent;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.selector
{
    /// <summary>
    /// Builds a config picker item: a title and, for a real config, the uppercased controller type.
    /// The config is resolved from the option id (its name) through an injected, live resolver, so the
    /// content stays correct as the config list changes. An empty id renders the italic "none" entry.
    /// </summary>
    public class ConfigComboItemContentBuilder : BaseComboItemContentBuilder
    {
        // Live lookup name -> config (null for "none" / unknown). Injected dependency, read at Build.
        private Func<string, UIGamepadConfig> _resolver;
        public ConfigComboItemContentBuilder Configs(Func<string, UIGamepadConfig> resolver)
        {
            this._resolver = resolver;
            return this;
        }

        public override ComboItemContentController Build()
        {
            string name = GetId();
            bool isNone = string.IsNullOrEmpty(name);
            UIGamepadConfig config = (!isNone && _resolver != null) ? _resolver(name) : null;

            string title = isNone
                ? ModLocalization.GetString("SteamInput_configNone")
                : (config != null ? config.Title : name);
            string typeLabel = config != null ? config.ControllerLabel : null;

            var root = new GameObject("ConfigItemContent", typeof(RectTransform));

            var layout = root.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(0, 0,
                Mathf.RoundToInt(SteamInputPalette.ComboItemPaddingV),
                Mathf.RoundToInt(SteamInputPalette.ComboItemPaddingV));
            layout.spacing = 1f;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            // Floor at the standard row height for the single-line ("none") case.
            var le = root.AddComponent<LayoutElement>();
            le.minHeight = ComboPalette.Height;

            var titleGo = new GameObject("Title", typeof(RectTransform));
            titleGo.transform.SetParent(root.transform, false);
            var titleText = UGUILabels.AddLabel(titleGo);
            titleText.text = title;
            titleText.fontSize = SteamInputPalette.ComboItemTitleFontSize;
            titleText.fontStyle = isNone ? FontStyles.Italic : FontStyles.Normal;
            titleText.alignment = TextAlignmentOptions.Left;

            if (!string.IsNullOrEmpty(typeLabel))
            {
                var typeGo = new GameObject("Type", typeof(RectTransform));
                typeGo.transform.SetParent(root.transform, false);
                var typeText = UGUILabels.AddLabel(typeGo);
                typeText.text = typeLabel.ToUpperInvariant();
                typeText.fontSize = SteamInputPalette.ComboItemTypeFontSize;
                typeText.color = SteamInputPalette.ComboItemTypeColor;
                typeText.alignment = TextAlignmentOptions.Left;
            }

            // The title color is applied by SetSelected, which the row calls at startup and on changes.
            return root
                .AddComponent<ConfigComboItemContentController>()
                .Title(titleText)
                .None(isNone);
        }
    }
}
