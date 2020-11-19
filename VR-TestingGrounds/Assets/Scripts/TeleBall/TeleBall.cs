using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class TeleBall : MonoBehaviour
{
    [SerializeField] private Player player = null;
    [SerializeField] private int[] ignoreLayers = null;
    [Space(10)]
    [SerializeField] private float teleFadeLength = 0.25f;

    private bool teleFade = false;

    /// <summary>
    /// Whether the player should be teleported to the next collision. Set to true when 
    /// thrown, and reset to false on first collision.
    /// </summary>
    public bool TeleportOnCollision { get; set; } = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (!TeleportOnCollision || teleFade) { return; }

        if (!IsIgnoredLayer(collision.gameObject.layer, ignoreLayers))
        {
            player.transform.position = collision.GetContact(0).point;
            TeleportOnCollision = false;
            StartCoroutine(DoSteamVRFade());
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
