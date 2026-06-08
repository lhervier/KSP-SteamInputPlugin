using UnityEngine;

namespace com.github.lhervier.ksp.ugui.shared
{
    public interface IUGUIBuilder<T> where T : MonoBehaviour
    {
        T Create();
    }
}