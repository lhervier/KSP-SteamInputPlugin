using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui.styles;
using com.github.lhervier.ksp.shared.ugui.sprites;
using com.github.lhervier.ksp.shared.ugui;
using com.github.lhervier.ksp.shared;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.menu
{
    /// <summary>
    /// The "Settings" entry at the bottom of the zones menu. A clickable row (label + chevron)
    /// that opens the settings view. Hover/click behave like the zone rows.
    /// </summary>
    public class SettingsItemBuilder : IUGUIBuilder<SettingsItemController>
    {
        // ====================================================
        // Builder parameters
        // ====================================================
        private CheatSheetViewModel _viewModel;
        public SettingsItemBuilder ViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        // ============================================
        // Build
        // ============================================

        public SettingsItemController Build()
        {
            var go = new GameObject("SettingsItem", typeof(RectTransform));
            SettingsItemController controller = go.AddComponent<SettingsItemController>();
            controller.ViewModel(_viewModel);

            // Match the zone rows' height so the entry lines up with them.
            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.minHeight = ButtonPalette.ButtonSize;

            // Background image: transparent normally, FieldBackground (#2a2a2a) on hover.
            // raycastTarget = true so pointer events fire on the whole row.
            var bgImage = go.AddComponent<Image>();
            bgImage.sprite = SpritesGlobal.FillSprite;
            bgImage.type = Image.Type.Simple;
            bgImage.color = Color.clear;
            bgImage.raycastTarget = true;

            // Horizontal: gear icon + label (greedy) + chevron pushed to the right
            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = DefaultPalette.Spacing;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            // Gear icon (white texture, tinted muted → accent on hover). Hidden if the texture is missing.
            var iconGo = new GameObject("Icon", typeof(RectTransform));
            iconGo.transform.SetParent(go.transform, false);

            var iconLayout = iconGo.AddComponent<LayoutElement>();
            iconLayout.preferredWidth = SteamInputPalette.MenuIconSize;
            iconLayout.preferredHeight = SteamInputPalette.MenuIconSize;
            iconLayout.minWidth = SteamInputPalette.MenuIconSize;
            iconLayout.minHeight = SteamInputPalette.MenuIconSize;

            var iconImage = iconGo.AddComponent<Image>();
            iconImage.sprite = SpritesZonesMenu.SettingsIconSprite;
            iconImage.color = SteamInputPalette.MenuIconColor;
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = false;
            if (iconImage.sprite == null)
            {
                iconGo.SetActive(false);
            }

            // PointerEnter/Exit highlight (row + icon) + PointerClick opens the settings, like ZoneRowBuilder.
            var trigger = go.AddComponent<EventTrigger>();

            var enterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enterEntry.callback.AddListener(_ => {
                bgImage.color = DefaultPalette.FieldBackgroundColor;
                iconImage.color = SteamInputPalette.MenuIconHoverColor;
            });
            trigger.triggers.Add(enterEntry);

            var exitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            exitEntry.callback.AddListener(_ => {
                bgImage.color = Color.clear;
                iconImage.color = SteamInputPalette.MenuIconColor;
            });
            trigger.triggers.Add(exitEntry);

            var clickEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
            clickEntry.callback.AddListener(_ => controller.OpenSettings());
            trigger.triggers.Add(clickEntry);

            // Label
            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(go.transform, false);

            // Greedy on width: consumes the leftover space and pushes the chevron to the right
            var labelLayout = labelGo.AddComponent<LayoutElement>();
            labelLayout.flexibleWidth = 1f;

            var label = labelGo.AddComponent<Text>();
            label.text = ModLocalization.GetString("SteamInput_settings");
            label.font = HighLogic.UISkin.font;
            label.fontSize = 12;
            label.color = DefaultPalette.LabelColor;
            label.alignment = TextAnchor.MiddleLeft;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.raycastTarget = false;

            // Chevron: hints that the entry opens another view (it does not toggle a value in place).
            // Centered in a button-sized box so it lines up with the zone rows' arrows column and
            // keeps the same right margin (instead of sticking to the menu border).
            var chevronGo = new GameObject("Chevron", typeof(RectTransform));
            chevronGo.transform.SetParent(go.transform, false);

            var chevronLayout = chevronGo.AddComponent<LayoutElement>();
            chevronLayout.preferredWidth = ButtonPalette.ButtonSize;
            chevronLayout.minWidth = ButtonPalette.ButtonSize;

            var chevron = chevronGo.AddComponent<Text>();
            chevron.text = "›";
            chevron.font = HighLogic.UISkin.font;
            chevron.fontSize = 12;
            chevron.color = SteamInputPalette.MenuTitleColor;
            chevron.alignment = TextAnchor.MiddleCenter;
            chevron.horizontalOverflow = HorizontalWrapMode.Overflow;
            chevron.verticalOverflow = VerticalWrapMode.Overflow;
            chevron.raycastTarget = false;

            return controller;
        }
    }
}
