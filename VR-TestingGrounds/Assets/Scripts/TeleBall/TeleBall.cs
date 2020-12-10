using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class TeleBall : MonoBehaviour
{
    [SerializeField] private Player player = null;
    [SerializeField] private Rigidbody rigBod = null;
    [SerializeField] private SteamVR_Action_Boolean returnInput = null;
    [SerializeField] private SteamVR_Input_Sources returnSource = SteamVR_Input_Sources.LeftHand;
    [SerializeField] private int[] ignoreLayers = null;
    [Space(10)]
    [SerializeField] private float teleFadeLength = 0.25f;

    private bool teleOnColl = false;
    private bool teleFade = false;
    private Vector3 initialOffsetFromPlayer;
    private float initialXZDistanceFromPlayer;

    /// <summary>
    /// Whether the player should be teleported to the next collision. Set to true when 
    /// thrown, and reset to false on first collision.
    /// </summary>
    public bool TeleportOnCollision
    {
        get { return teleOnColl; }
        set
        {
            if (value) { rigBod.useGravity = true; }
            teleOnColl = value;
        }
    }

    private void Start()
    {
        initialOffsetFromPlayer = transform.position - player.transform.position;
        initialXZDistanceFromPlayer = Vector3.Magnitude(new Vector3(initialOffsetFromPlayer.x, 0, initialOffsetFromPlayer.z));
    }
    private void Update()
    {
        if (returnInput.GetStateDown(returnSource))
        {
            rigBod.velocity = Vector3.zero;

            if (returnInput.GetStateDown(returnSource))
            {
                transform.position = player.headCollider.transform.position +
                    Vector3.ProjectOnPlane(player.bodyDirectionGuess, Vector3.up).normalized * initialXZDistanceFromPlayer;
                rigBod.angularVelocity = Vector3.zero;
                rigBod.useGravity = false;
                TeleportOnCollision = false;
            }
        }
        else if (returnInput.GetStateUp(returnSource))
        {
            rigBod.useGravity = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!TeleportOnCollision || teleFade) { return; }

        if (!IsIgnoredLayer(collision.gameObject.layer, ignoreLayers))
        {
            var contactPoint = collision.GetContact(0);
            if (Vector3.Dot(Vector3.up, contactPoint.normal) >= 0.25f)
            {
                player.transform.position = contactPoint.point;
                TeleportOnCollision = false;
                StartCoroutine(DoSteamVRFade());
            }
        }
    }

    private bool IsIgnoredLayer(int checkLayer, int[] ignoredLayers)
    {
        return Array.Exists(ignoredLayers, ignoredLayer => ignoredLayer == checkLayer);
    }

    private IEnumerator DoSteamVRFade()
    {
        teleFade = true;
        SteamVR_Fade.Start(Color.black, teleFadeLength, true);

        yield return new WaitForSecondsRealtime(teleFadeLength);

        SteamVR_Fade.Start(Color.clear, teleFadeLength, true);
        teleFade = false;
    }
}
