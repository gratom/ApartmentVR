using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyVRMenu;

public class MaterialGroupMenuItemContainer : BaseMenuItemContainer
{
    public override MenuItem Init(MonoBehaviour AttachedObject)
    {
        MenuItem copyOfMenuItem = base.Init(AttachedObject);
        if ((AttachedObject as MaterialGroup) != null)
        {
            if(copyOfMenuItem.VisibleName)
            {
                copyOfMenuItem.VisibleName.text = ((MaterialGroup)AttachedObject).nameOfMaterialGroup.Replace(' ', '\n');
            }
        }
        else
        {
            Debug.Log("AttachedObject not instanceof MaterialGroup");
        }
        return copyOfMenuItem;
    }
}
