using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for basic interactive objects
/// </summary>
public class InteractiveObject : MonoBehaviour, IBaseAction
{

    /// <summary>
    /// Delegate, that should called, when the pointer left from this object
    /// </summary>
    public SimpleActionDelegate OnPointerLeft
    {
        get
        {
            if (_onPointerLeft != null)
            {
                return _onPointerLeft;
            }
            else
            {
                return () => { };
            }
        }
        set
        {
            _onPointerLeft = value;
        }
    }
    private SimpleActionDelegate _onPointerLeft;  

    /// <summary>
    /// Delegate, that should called, when the pointer hover on this object
    /// </summary>
    public SimpleActionDelegate OnPointerHover
    {
        get
        {
            if (_onPointerHover != null)
            {
                return _onPointerHover;
            }
            else
            {
                return () => { };
            }
        }
        set
        {
            _onPointerHover = value;
        }
    }
    private SimpleActionDelegate _onPointerHover;

    /// <summary>
    /// Delegate, that shold be called, when you need to take an active action
    /// </summary>
    public SimpleActionDelegate OnActiveAction
    {
        get
        {
            if (_onActiveAction != null)
            {
                return _onActiveAction;
            }
            else
            {
                return () => { };
            }
        }
        set
        {
            _onActiveAction = value;
        }
    }
    private SimpleActionDelegate _onActiveAction;

}
