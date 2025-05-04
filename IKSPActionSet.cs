using UnityEngine;
using System;
using System.Collections;

namespace com.github.lhervier.ksp 
{
    public interface IKspActionSet {

        // <summary>    
        //  The name of the control set
        // </summary>
        string ControlName();

        // <summary>
        //  Whether the control set is active
        // </summary>
        RefreshType Active();

        // <summary>
        //  Whether the control set is the default
        // </summary>
        bool Default();
    }
}