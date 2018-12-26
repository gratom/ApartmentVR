using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LineEffects : MonoBehaviour
{
    public bool isEffect { get; protected set; }

    [SerializeField]
    protected GameObject effectGameObject;

    public override string ToString()
    {
        return gameObject.name + "; play effect = " + isEffect + "; attached to " + gameObject.transform.parent.name;
    }

    public abstract void PlayEffect();

    public abstract void StopEffect();
}
