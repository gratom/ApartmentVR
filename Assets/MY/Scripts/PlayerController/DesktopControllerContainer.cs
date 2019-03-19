using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyPlayerControllers;

public class DesktopControllerContainer : BasePlayerControllerContainer
{

    [SerializeField]
    private string TelepostSurfaceTag;

    private GameObject TeleportSurfaceGameObject;

    public override void Init()
    {
        spawnAction = () =>
        {

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            TeleportSurfaceGameObject = GameObject.FindGameObjectWithTag(TelepostSurfaceTag);
            if (TeleportSurfaceGameObject != null)
            {
                Destroy(TeleportSurfaceGameObject);
            }
            base.Init();
        };
    }

}

