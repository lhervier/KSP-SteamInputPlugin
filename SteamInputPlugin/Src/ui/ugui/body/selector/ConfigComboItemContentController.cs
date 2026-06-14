using TMPro;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.shared.ugui.combo.itemcontent;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.selector
{
    /// <summary>
    /// Combo item content for a controller config: a title line (plus an optional controller-type
    /// line built by its builder). Reflects selection by recoloring the title; the "none" placeholder
    /// keeps its dimmed color when not selected.
    /// </summary>
    public class ConfigComboItemContentController : ComboItemContentController
    {
        private TextMeshProUGUI _title;
        public ConfigComboItemContentController Title(TextMeshProUGUI title)
        {
            this._title = title;
            return this;
        }

        // The "none" placeholder uses a dimmed color in its unselected state.
        private bool _none;
        public ConfigComboItemContentController None(bool none)
        {
            this._none = none;
            return this;
        }

        public override void SetSelected(bool selected)
        {
            if (_title == null) return;
            _title.color = selected
                ? SteamInputPalette.ComboItemTitleSelectedColor
                : (_none ? SteamInputPalette.ComboItemNoneColor : SteamInputPalette.ComboItemTitleColor);
        }
    }
}
