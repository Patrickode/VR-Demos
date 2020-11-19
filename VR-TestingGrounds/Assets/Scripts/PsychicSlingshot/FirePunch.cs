using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class FirePunch : MonoBehaviour
{
    [SerializeField] private SteamVR_Action_Boolean punchInput = null;
    [Tooltip("The source to take input from. Must be left or right hand.")]
    [SerializeField] private HandPhysics handColliderSource = null;
    [SerializeField] private ParticleSystem handFireParticles = null;
    [SerializeField] private int firePunchLayer = 10;

    private HandCollider handCollider;
    private int originalLayer;

    private void Start()
    {
        handCollider = handColliderSource.handCollider;
        originalLayer = handCollider.gameObject.layer;
        punchInput[handColliderSource.hand.handType].onChange += ToggleFirePunch;
    }

    private void ToggleFirePunch(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource,
        bool newState)
    {
        if (newState)
        {
            handFireParticles.Play();
            SetLayerRecursively(handCollider.gameObject, firePunchLayer);
        }
        else
        {
            handFireParticles.Stop();
            SetLayerRecursively(handCollider.gameObject, originalLayer);
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
