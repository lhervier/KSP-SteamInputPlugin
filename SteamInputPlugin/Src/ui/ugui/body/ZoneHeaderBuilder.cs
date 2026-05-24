using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.ugui.styles;
using com.github.lhervier.ksp.ui.model;
using System;

namespace com.github.lhervier.ksp.ui.ugui.body
{
    /// <summary>
    /// Displays one UIPhysicalZone:
    ///   - Header row with the zone label (e.g. "STICK GAUCHE")
    ///   - "NORMAL" section if the zone has a GroupId
    ///   - "↓ MODESHIFT" section if the zone has a ModeshiftGroupId
    /// Styled to match the mockup .kzone / .kzh / .kstate rules.
    /// </summary>
    public class ZoneHeaderBuilder
    {
        private const int BodyPaddingHorizontal = 8;
        private const float ZoneSeparatorHeight = 1f;

        private CheatSheetViewModel _viewModel;

        public ZoneHeaderBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public ZoneHeaderController Create(UIPresetZone zone)
        {
            var go = new GameObject("Header", typeof(RectTransform));
            ZoneHeaderController controller = go.AddComponent<ZoneHeaderController>();
            controller.Initialize(_viewModel);

            // Fixed header height (matches .kzh from the mockup)
            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.minHeight = SteamInputPalette.MainZoneHeaderHeight;
            layoutElement.preferredHeight = SteamInputPalette.MainZoneHeaderHeight;

            // Sliced chrome: dark fill with 1px lines at top and bottom (no left/right borders).
            // The colors are baked into the sprite, so the Image's color stays white.
            var bgImage = go.AddComponent<Image>();
            bgImage.sprite = SpritesPhysicalZone.HeaderChromeSprite;
            bgImage.type = Image.Type.Sliced;
            bgImage.color = Color.white;
            bgImage.raycastTarget = false;

            // Inner HLG carries the horizontal/vertical padding around the title text
            var hlg = go.AddComponent<HorizontalLayoutGroup>();
            hlg.padding = new RectOffset(
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingLeft),
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingRight),
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingTop),
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingBottom)
            );
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = true;
            hlg.childForceExpandHeight = true;

            var titleGo = new GameObject("Title", typeof(RectTransform));
            titleGo.transform.SetParent(go.transform, false);

            var label = titleGo.AddComponent<Text>();
            label.text = zone.Label;
            label.font = HighLogic.UISkin.font;
            label.fontSize = 12;
            label.fontStyle = FontStyle.Bold;
            label.color = SteamInputPalette.ZoneName;
            label.alignment = TextAnchor.MiddleLeft;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;
            controller.BindLabel(label);

            return controller;
        }

        public class ZoneHeaderController : BaseSteamInputController
        {
            private Text _label;

            public void BindLabel(Text label)
            {
                _label = label;
            }

            public void UpdateZone(UIPresetZone zone)
            {
                _label.text = zone.Label;
            }
        }
    }
}
