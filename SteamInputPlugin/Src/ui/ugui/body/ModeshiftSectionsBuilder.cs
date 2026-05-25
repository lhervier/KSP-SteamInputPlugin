using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.ui.styles;
using com.github.lhervier.ksp.ui.model;
using System.Collections.Generic;

namespace com.github.lhervier.ksp.ui.ugui.body
{
    public class ModeshiftSectionsBuilder
    {
        private CheatSheetViewModel _viewModel;
        private ZoneSectionBuilder _zoneSectionBuilder;
        public ModeshiftSectionsBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._zoneSectionBuilder = new ZoneSectionBuilder(viewModel);
        }

        public ModeshiftSectionsController Create(UIPresetZone zone)
        {
            var go = new GameObject("ModeshiftSections", typeof(RectTransform));
            ModeshiftSectionsController controller = go.AddComponent<ModeshiftSectionsController>();
            controller.Initialize(_viewModel);
            controller.BindZoneSectionBuilder(_zoneSectionBuilder);

            // Horizontal padding (Option A: padding on the container, not per-section)
            // Vertical padding-bottom matches the .kzone body breathing room.
            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.spacing = SteamInputPalette.MainSectionSpacing;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            controller.UpdateZone(zone);

            return controller;
        }

        public class ModeshiftSectionsController : BaseSteamInputController
        {
            ZoneSectionBuilder _builder;
            Dictionary<string, ZoneSectionBuilder.ZoneSectionController> _sectionControllers = new Dictionary<string, ZoneSectionBuilder.ZoneSectionController>();

            public void BindZoneSectionBuilder(ZoneSectionBuilder builder)
            {
                _builder = builder;
            }

            public void UpdateZone(UIPresetZone zone)
            {
                // 1. Destroy controllers for groupIds no longer present.
                // Collect the keys first; we can't remove from a Dictionary while iterating its Keys.
                var toRemove = new List<string>();
                foreach (string groupId in _sectionControllers.Keys)
                {
                    if (!zone.ModeshiftGroupIds.Contains(groupId))
                    {
                        toRemove.Add(groupId);
                    }
                }
                foreach (string groupId in toRemove)
                {
                    Destroy(_sectionControllers[groupId].gameObject);
                    _sectionControllers.Remove(groupId);
                }

                // 2. Create or update controllers, then impose the visual order via SetSiblingIndex.
                // Order is driven by the input list, not by the dictionary's iteration order.
                for (int i = 0; i < zone.ModeshiftGroupIds.Count; i++)
                {
                    string groupId = zone.ModeshiftGroupIds[i];
                    ZoneSectionBuilder.ZoneSectionController controller = GetController(groupId);
                    controller.UpdateGroupId(groupId);
                    controller.transform.SetSiblingIndex(i);
                }
            }

            public ZoneSectionBuilder.ZoneSectionController GetController(string groupId)
            {
                if (!_sectionControllers.TryGetValue(groupId, out ZoneSectionBuilder.ZoneSectionController sectionController))
                {
                    sectionController = this._builder.Create(groupId, true);
                    sectionController.transform.SetParent(gameObject.transform);
                    _sectionControllers[groupId] = sectionController;
                    return sectionController;
                }
                return sectionController;
            }
        }
    }
}
