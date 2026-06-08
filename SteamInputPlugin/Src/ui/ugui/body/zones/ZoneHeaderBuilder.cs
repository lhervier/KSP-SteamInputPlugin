using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using com.github.lhervier.ksp.steaminput.ui.model;
using com.github.lhervier.ksp.shared.ugui.styles;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.zones
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
        private CheatSheetViewModel _viewModel;

        public ZoneHeaderBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        public ZoneHeaderController Create(UIPhysicalZone zone)
        {
            var go = new GameObject("Header", typeof(RectTransform));
            ZoneHeaderController controller = go.AddComponent<ZoneHeaderController>();
            controller.Initialize(_viewModel);

            // Fixed header height (matches .kzh from the mockup)
            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.minHeight = SteamInputPalette.ZoneHeaderHeight;
            layoutElement.preferredHeight = SteamInputPalette.ZoneHeaderHeight;

            // Sliced chrome: dark fill with 1px lines at top and bottom (no left/right borders).
            // The colors are baked into the sprite, so the Image's color stays white.
            var bgImage = go.AddComponent<Image>();
            bgImage.sprite = SpritesZones.ZoneHeaderChromeSprite;
            bgImage.type = Image.Type.Sliced;
            bgImage.color = Color.white;
            bgImage.raycastTarget = false;

            // Inner HLG carries the horizontal/vertical padding around the title text
            var hlg = go.AddComponent<HorizontalLayoutGroup>();
            hlg.padding = new RectOffset(
                Mathf.RoundToInt(DefaultPalette.PaddingLeft),
                Mathf.RoundToInt(DefaultPalette.PaddingRight),
                Mathf.RoundToInt(DefaultPalette.PaddingTop),
                Mathf.RoundToInt(DefaultPalette.PaddingBottom)
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
            label.color = SteamInputPalette.ZoneNameColor;
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

            public void UpdateZone(UIPhysicalZone zone)
            {
                _label.text = zone.Label;
            }
        }
    }
}
