using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ParentToHandModel : MonoBehaviour
{
    [SerializeField] private Hand hand = null;
    private bool parented = false;

    private void Update()
    {
        if (!parented)
        {
            if (hand && hand.mainRenderModel)
            {
                transform.parent = hand.mainRenderModel.transform;
                parented = true;
            }
        }
    }
}
