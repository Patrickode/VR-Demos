using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EpsilonOffset : MonoBehaviour
{
    [SerializeField] [Range(-1, 1)] private int offsetX = 0;
    [SerializeField] [Range(-1, 1)] private int offsetY = 0;
    [SerializeField] [Range(-1, 1)] private int offsetZ = 0;
    [Space(5)]
    [SerializeField] private float offsetAmount = 0.0001f;

    private void Start()
    {
        Vector3 offsetPos = transform.position;
        offsetPos.x += offsetX != 0 ? offsetAmount * offsetX : 0;
        offsetPos.y += offsetY != 0 ? offsetAmount * offsetY : 0;
        offsetPos.z += offsetZ != 0 ? offsetAmount * offsetZ : 0;
        transform.position = offsetPos;
    }
}
