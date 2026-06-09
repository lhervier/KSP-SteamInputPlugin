using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.steaminput.ui.styles;
using com.github.lhervier.ksp.steaminput.ui.model;
using System.Collections.Generic;
using com.github.lhervier.ksp.shared.ugui.styles;
using com.github.lhervier.ksp.shared.ugui;

namespace com.github.lhervier.ksp.steaminput.ui.ugui.body.zones
{
    public class SectionController : MonoBehaviour
    {
        
        private MouseLineBuilder.MouseLineController _mouseLineController;
        private readonly List<ActivatorBuilder.ActivatorController> _rowControllers
            = new List<ActivatorBuilder.ActivatorController>();

        // =========================================
        // Life cycle
        // =========================================

        private CheatSheetViewModel _viewModel;
        public SectionController ViewModel(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            return this;
        }

        private GameObject _headerLabel;
        public SectionController HeaderLabel(GameObject headerLabel)
        {
            this._headerLabel = headerLabel;
            return this;
        }

        private string _groupId;
        public SectionController GroupId(string groupId)
        {
            this._groupId = groupId;
            return this;
        }

        public void Start()
        {
            this.UpdateGroupId(this._groupId);
        }

        // ==========================================
        // Public API
        // ==========================================

        /// <summary>Show/hide the "NORMAL" / "↓ MODESHIFT" subheader (kept at sibling index 0).</summary>
        public void SetHeaderVisible(bool visible)
        {
            if( _headerLabel != null )
            {
                _headerLabel.SetActive(visible);
            }
        }

        public void UpdateGroupId(string groupId)
        {
            _groupId = groupId;
            
            // Rebuild the section content below the subheader (first child).
            foreach( ActivatorBuilder.ActivatorController row in _rowControllers )
            {
                Destroy(row.gameObject);
            }
            _rowControllers.Clear();
            if( _mouseLineController != null )
            {
                Destroy(_mouseLineController.gameObject);
                _mouseLineController = null;
            }

            // Mouse-mode groups get a banner right after the subheader, above any rows.
            if( _viewModel.IsMouseGroup(groupId) )
            {
                _mouseLineController = new MouseLineBuilder().Build();
                _mouseLineController.transform.SetParent(gameObject.transform, false);
                _mouseLineController.transform.SetSiblingIndex(1);
            }

            // Then one row per activator (e.g. a click on the joystick).
            foreach( UIActivator activator in _viewModel.GetActivators(groupId) )
            {
                ActivatorBuilder.ActivatorController row = new ActivatorBuilder()
                    .ViewModel(_viewModel)
                    .Activator(activator)
                    .Build();
                row.transform.SetParent(gameObject.transform, false);
                row.transform.SetAsLastSibling();
                _rowControllers.Add(row);
            }
        }
    }
}
