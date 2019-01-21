using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyPlayerControllers;

public class VRControllerContainer : PlayerControllerContainer
{

    [SerializeField]
    private GameObject TeleportingSystem;

    [SerializeField]
    private string TelepostSurfaceTag;

    private GameObject TeleportSurfaceGameObject;

    public override void Init()
    {
        spawnAction = () =>
        {
            if (TeleportingSystem != null)
            {
                Instantiate(TeleportingSystem);
                TeleportSurfaceGameObject = GameObject.FindGameObjectWithTag(TelepostSurfaceTag);
                if (TeleportSurfaceGameObject != null)
                {
                    TeleportSurfaceGameObject.AddComponent<Valve.VR.InteractionSystem.TeleportArea>();
                }
                else
                {
                    Debug.LogError("You have not correct surface for teleporting system!");
                }
            }
            else
            {
                Debug.LogError("TeleportintSystem is can not be null!");
            }
            base.Init();
        };
    }

}
