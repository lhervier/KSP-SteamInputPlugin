using UnityEngine;
using System;
using System.Collections;

namespace com.github.lhervier.ksp 
{
    public interface IKspActionSet {
        string ControlName();
        bool Active();
    }
}