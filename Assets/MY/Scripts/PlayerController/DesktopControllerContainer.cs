using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyPlayerControllers;

public class DesktopControllerContainer : BasePlayerControllerContainer
{

    [SerializeField]
    private string TelepostSurfaceTag;

    [SerializeField]
    private bool CanChangePostProcessing;

    private GameObject TeleportSurfaceGameObject;

    public override void Init()
    {
        spawnAction = () =>
        {
            ChangePostProcessing(CanChangePostProcessing);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            TeleportSurfaceGameObject = GameObject.FindGameObjectWithTag(TelepostSurfaceTag);
            if (TeleportSurfaceGameObject != null)
            {
                Destroy(TeleportSurfaceGameObject);
            }
        };
        base.Init();
    }

}

