using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DetectObjExit : MonoBehaviour
{
    [Tooltip("When one of these objects leaves / is returned to this object's trigger, it will be detected.")]
    [SerializeField] private Collider[] watchedObjects = null;
    private HashSet<Collider> watchedObjSet = null;

    public static Action<Collider> ObjectExit;
    public static Action<Collider> ObjectReturn;

    private void Start()
    {
        transform.parent = null;
        watchedObjSet = new HashSet<Collider>(watchedObjects);
    }

    private void OnTriggerExit(Collider other)
    {
        if (watchedObjSet.Contains(other))
        {
            ObjectExit?.Invoke(other);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (watchedObjSet.Contains(other))
        {
            ObjectReturn?.Invoke(other);
        }
    }
}
