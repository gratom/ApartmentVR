using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChoosingEffect : InteractiveObject
{

    public GameObject ObjectForSelection;

    private void Awake()
    {
        OnPointerHover = () => 
        {
            if (!ObjectForSelection.activeSelf)
            {
                ObjectForSelection.SetActive(true);
            }
        };
        OnPointerLeft = () => 
        {
            if (ObjectForSelection.activeSelf)
            {
                ObjectForSelection.SetActive(false);
            }
        };
    }

}
