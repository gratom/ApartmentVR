using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEffect : MonoBehaviour
{

    public delegate void EffectAction();

    /// <summary>
    /// Delegate, that called, when the pointer left from this object
    /// </summary>
    public EffectAction OnPointerLeft
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
    private EffectAction _onPointerLeft;
    

    /// <summary>
    /// Delegate, that called, when the pointer hover on this object
    /// </summary>
    public EffectAction OnPointerHover
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
    private EffectAction _onPointerHover;

}
