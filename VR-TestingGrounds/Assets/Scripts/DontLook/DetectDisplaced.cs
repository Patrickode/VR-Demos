using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DetectDisplaced : MonoBehaviour
{
    [SerializeField] private Camera detectCam = null;
    /// <summary>
    /// The list of colliders that have been displaced.
    /// </summary>
    private List<Collider> displacedColls;
    /// <summary>
    /// The list of displaced colliders that we've detected.
    /// </summary>
    private List<Collider> detectedColls;
    /// <summary>
    /// The list of detected colliders that are no longer displaced, but we haven't seen returned yet.
    /// </summary>
    private List<Collider> unseenReturnedColls;

    private bool anyCollsDetected = false;

    public static Action DisplacementDetected;
    public static Action DisplacementAverted;

    private void Start()
    {
        displacedColls = new List<Collider>();
        detectedColls = new List<Collider>();
        unseenReturnedColls = new List<Collider>();

        DetectObjExit.ObjectExit += AddDisplacedColl;
        DetectObjExit.ObjectReturn += RemoveDisplacedColl;
    }
    private void OnDestroy()
    {
        DetectObjExit.ObjectExit += AddDisplacedColl;
        DetectObjExit.ObjectReturn += RemoveDisplacedColl;
    }

    private void AddDisplacedColl(Collider coll) { displacedColls.Add(coll); }
    private void RemoveDisplacedColl(Collider coll)
    {
        displacedColls.Remove(coll);

        //If a deteced coll was successfully removed from the detected list,
        if (detectedColls.Remove(coll))
        {
            //add it to the unseen list until it's seen; we don't know if it's returned until we see it.
            unseenReturnedColls.Add(coll);
        }
    }

    private void Update()
    {
        //If there are no colls in either of these lists, we don't need to try and detect any.
        if (displacedColls.Count <= 0 && unseenReturnedColls.Count <= 0) { return; }

        //If we haven't detected any colls yet, and we just detected a displaced coll,
        if (CollsInDetectCamView(displacedColls, out var collsDetected))
        {
            //Add any new colls detected to the list of detected colls.
            detectedColls = detectedColls.Union(collsDetected).ToList();

            //If we haven't already, note that we've detected a displaced coll.
            if (!anyCollsDetected)
            {
                anyCollsDetected = true;
                DisplacementDetected?.Invoke();
            }
        }

        //If we can see any colls that were once displaced, but are now returned,
        if (CollsInDetectCamView(unseenReturnedColls, out var unseensDetected))
        {
            //remove them from the unseen returned coll list, because we just saw they were returned.
            foreach (Collider coll in unseensDetected)
            {
                unseenReturnedColls.Remove(coll);
            }
        }

        //If any colls have been detected, but all of them have been returned and seen to be returned,
        //avert the detection.
        if (anyCollsDetected && detectedColls.Count <= 0 && unseenReturnedColls.Count <= 0)
        {
            anyCollsDetected = false;
            DisplacementAverted?.Invoke();
        }
    }

    private bool CollsInDetectCamView(List<Collider> collsToCheck)
    {
        //Get the frustum planes of this camera, and go through all of the colls to check.
        Plane[] camPlanes = GeometryUtility.CalculateFrustumPlanes(detectCam);
        foreach (Collider coll in displacedColls)
        {
            //If any coll is inside the detect camera's frustum,
            if (GeometryUtility.TestPlanesAABB(camPlanes, coll.bounds))
            {
                //check to see if the coll is obstructed, and thus not visible, by casting a small sphere at it.
                bool castSuccess = Physics.SphereCast(
                    detectCam.transform.position,
                    0.05f,
                    coll.transform.position - detectCam.transform.position,
                    out RaycastHit hit
                );

                //If we hit something and that something is the collider, we've detected a displaced thing.
                return castSuccess && hit.collider.Equals(coll);
            }
        }

        return false;
    }
    private bool CollsInDetectCamView(List<Collider> collsToCheck, out List<Collider> collsDetected)
    {
        collsDetected = new List<Collider>();

        //Get the frustum planes of this camera, and go through all of the colls to check.
        Plane[] camPlanes = GeometryUtility.CalculateFrustumPlanes(detectCam);
        foreach (Collider coll in displacedColls)
        {
            if (!coll) { continue; }

            //If any coll is inside the detect camera's frustum,
            if (GeometryUtility.TestPlanesAABB(camPlanes, coll.bounds))
            {
                //check to see if the coll is obstructed, and thus not visible, by casting a small sphere at it.
                bool castSuccess = Physics.SphereCast(
                    detectCam.transform.position,
                    0.05f,
                    coll.transform.position - detectCam.transform.position,
                    out RaycastHit hit
                );

                //If we hit something and that something is the collider, we've detected a displaced thing.
                if (castSuccess && hit.collider.Equals(coll)) { collsDetected.Add(coll); }
            }
        }

        //If nothing was found, set detectedColls to null. Return true if detected colls is not null.
        if (collsDetected.Count <= 0) { collsDetected = null; }
        return collsDetected != null;
    }
}