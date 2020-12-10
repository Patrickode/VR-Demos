using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserJitter : MonoBehaviour
{
    [SerializeField] private float maxJitterAmount = 1f;
    private Quaternion originalOrientation;

    private void Start()
    {
        originalOrientation = transform.localRotation;
    }

    private void Update()
    {
        transform.localRotation = originalOrientation;
        transform.localRotation *= Quaternion.AngleAxis(
            Random.Range(-maxJitterAmount, maxJitterAmount),
            Vector3.right
        );
        transform.localRotation *= Quaternion.AngleAxis(
            Random.Range(-maxJitterAmount, maxJitterAmount),
            Vector3.up
        );
    }
}
