using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotate : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 1f;

    private void Update()
    {
        transform.localRotation *= Quaternion.AngleAxis(rotateSpeed, Vector3.up);
    }
}
