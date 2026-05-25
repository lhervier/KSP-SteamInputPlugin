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
    public class ZoneBodyBuilder
    {
        private CheatSheetViewModel _viewModel;
        private ZoneSectionBuilder _zoneSectionBuilder;
        private ModeshiftSectionsBuilder _modeshiftZoneSectionsBuilder;

        public ZoneBodyBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._zoneSectionBuilder = new ZoneSectionBuilder(viewModel);
            this._modeshiftZoneSectionsBuilder = new ModeshiftSectionsBuilder(viewModel);
        }

        public ZoneBodyController Create(UIPresetZone zone)
        {
            var go = new GameObject("Body", typeof(RectTransform));
            ZoneBodyController controller = go.AddComponent<ZoneBodyController>();
            controller.Initialize(_viewModel);
            controller.BindZoneSectionBuilder(_zoneSectionBuilder);
            controller.BindModeshiftSectionsBuilder(_modeshiftZoneSectionsBuilder);

            // Horizontal padding (Option A: padding on the container, not per-section)
            // Vertical padding-bottom matches the .kzone body breathing room.
            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingLeft),
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingRight),
                0,
                Mathf.RoundToInt(SteamInputPalette.DefaultPaddingBottom)
            );
            layout.spacing = SteamInputPalette.ZoneSectionSpacing;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            controller.UpdateZone(zone);

            return controller;
        }

        public class ZoneBodyController : BaseSteamInputController
        {
            private ZoneSectionBuilder _zoneSectionBuilder;
            private ModeshiftSectionsBuilder _modeshiftSectionsBuilder;
            private ZoneSectionBuilder.ZoneSectionController _normalSectionController;
            private ModeshiftSectionsBuilder.ModeshiftSectionsController _modeshiftSectionsController;

            public void BindZoneSectionBuilder(ZoneSectionBuilder builder)
            {
                this._zoneSectionBuilder = builder;
            }
            public void BindModeshiftSectionsBuilder(ModeshiftSectionsBuilder modeshiftSectionsBuilder)
            {
                this._modeshiftSectionsBuilder = modeshiftSectionsBuilder;
            }

            public void UpdateZone(UIPresetZone zone)
            {
                this.UpdateNormalSection(zone);
                this.UpdateModeshiftSections(zone);
            }

            public void UpdateNormalSection(UIPresetZone zone)
            {
                // No normal section
                if( zone.GroupId == null )
                {
                    if( _normalSectionController != null )
                    {
                        Destroy(_normalSectionController.gameObject);
                        _normalSectionController = null;
                    }
                    return;
                }

                // No existing normal section => Create it
                if( _normalSectionController == null )
                {
                    _normalSectionController = this._zoneSectionBuilder.Create(zone.GroupId, false);
                    _normalSectionController.transform.SetParent(gameObject.transform);
                    _normalSectionController.transform.SetAsFirstSibling();
                    return;
                }

                // Existing normal section => Update it
                _normalSectionController.UpdateGroupId(zone.GroupId);
            }

            public void UpdateModeshiftSections(UIPresetZone zone)
            {
                // No existing modeshift sections => Create them
                if( _modeshiftSectionsController == null )
                {
                    _modeshiftSectionsController = this._modeshiftSectionsBuilder.Create(zone);
                    _modeshiftSectionsController.transform.SetParent(gameObject.transform);
                    _modeshiftSectionsController.transform.SetAsLastSibling();
                    return;
                } 

                // Existing modeshift sections => Update them
                _modeshiftSectionsController.UpdateZone(zone);
            }
        }
    }
}
