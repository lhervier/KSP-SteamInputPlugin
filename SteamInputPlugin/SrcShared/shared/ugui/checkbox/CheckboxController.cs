using UnityEngine;

namespace com.github.lhervier.ksp.shared.ugui.checkbox
{
    public class CheckboxController : MonoBehaviour
    {
        private GameObject _checkMark;

        public void BindCheckmark(GameObject checkmark)
        {
            _checkMark = checkmark;
        }

        public bool IsChecked()
        {
            if( _checkMark == null ) return false;
            return _checkMark.activeInHierarchy;
        }

        public void SetChecked(bool isChecked)
        {
            _checkMark.SetActive(isChecked);
        }
    }
}
