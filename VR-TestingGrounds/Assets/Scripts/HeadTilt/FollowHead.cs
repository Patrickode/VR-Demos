using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHead : MonoBehaviour
{
    [SerializeField] private GameObject head = null;

    private void Update()
    {
        Vector3 headUpXZ = Vector3.ProjectOnPlane(head.transform.up, Vector3.up);
        Vector3 axis = Vector3.Cross(Vector3.up, headUpXZ);
        float headAngleFromUp = Vector3.SignedAngle(Vector3.up, head.transform.up, axis);
        transform.rotation = Quaternion.AngleAxis(headAngleFromUp, axis);
    }
}
