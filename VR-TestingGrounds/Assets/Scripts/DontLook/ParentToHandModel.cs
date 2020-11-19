using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ParentToHandModel : MonoBehaviour
{
    [SerializeField] private Hand hand = null;

    private void Start()
    {
        transform.parent = hand.mainRenderModel.transform;
    }
}
