using System.Collections;
using System.Collections.Generic;
using MyVRMenu;
using UnityEngine;

public class MaterialMenuItemContainer : BaseMenuItemContainer
{   

    public override MenuItem Init(MonoBehaviour AttachedObject)
    {        
        MenuItem copyOfMenuItem = base.Init(AttachedObject);
        if ((AttachedObject as LoadedMaterial) != null)
        {            
            if (copyOfMenuItem.VisibleName)
            {
                copyOfMenuItem.VisibleName.text = ((LoadedMaterial)AttachedObject).LoadedMaterialName;
            }
        }
        else
        {
            Debug.Log("AttachedObject not instanceof LoadedMaterial");
        }
        return copyOfMenuItem;
    }

}