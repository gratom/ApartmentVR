using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class VRLineShowingController : MonoBehaviour
{

    public Hand hand;
    public PointerEffect pointerEffect;

    private bool isLoading = true;

    private void Update()
    {
        if (SteamVR_Input._default.inActions.Teleport.GetState(hand.handType))
        {
            pointerEffect.IsShowing = false;
        }
        else
        {
            pointerEffect.IsShowing = true;
        }

        if (SteamVR_Input._default.inActions.GrabPinch.GetStateDown(hand.handType) && isLoading)
        {
            pointerEffect.lastBaseEffect.gameObject.GetComponent<SimpleAction>().simpleActionDelegate();
            isLoading = false;
        }

    }

}
