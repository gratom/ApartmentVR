using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MenuSecondLineEffects : LineEffects
{
    public override void PlayEffect()
    {
        effectGameObject.SetActive(true);
    }

    public override void StopEffect()
    {
        effectGameObject.SetActive(false);
    }
}
