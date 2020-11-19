using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class StayOnBack : MonoBehaviour
{
    [SerializeField] private Player player = null;
    [Space(10)]
    [Tooltip("1.0f = 100%.")]
    [SerializeField] private float percentEyeHeight = 0.75f;
    [SerializeField] private float offsetFromBack = 0f;

    private void Update()
    {
        Vector3 normalBodDirection = player.bodyDirectionGuess.normalized;
        transform.rotation = Quaternion.LookRotation(normalBodDirection, player.transform.up);

        Vector3 updatedPos = player.feetPositionGuess;
        updatedPos -= normalBodDirection * (transform.localScale.z / 2);
        updatedPos += normalBodDirection * offsetFromBack;

        updatedPos.y += player.eyeHeight * percentEyeHeight;

        transform.position = updatedPos;
    }
}