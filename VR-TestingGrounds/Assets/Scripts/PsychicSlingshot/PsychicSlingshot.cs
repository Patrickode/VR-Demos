using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class PsychicSlingshot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SteamVR_Action_Boolean slingshotInput = null;
    [SerializeField] private SteamVR_Action_Boolean pointerInput = null;
    [Tooltip("The source to take input from. Must be left or right hand.")]
    [SerializeField] private SteamVR_Input_Sources inputSource = SteamVR_Input_Sources.Any;
    [SerializeField] private Transform handTransform = null;
    [SerializeField] private LineRenderer pointerLine = null;
    [SerializeField] private LineRenderer targetLine = null;
    [SerializeField] private LineRenderer slingshotLine = null;
    [SerializeField] private LineRenderer directionLine = null;

    [Header("Parameters")]
    [Tooltip("The maximum amount the slingshot can be pulled. Pulling further than max distance will be no " +
        "stronger than pulling the max distance.")]
    [SerializeField] private float maxPullDistance = 5;
    [SerializeField] private float maxForce = 50;
    [SerializeField] private float maxGrabRange = 25;
    [Tooltip("How lenient pointing and grabbing an object is. Specifically, the radius of the spherecast " +
        "for detecting things to grab.")]
    [SerializeField] private float targetAccuracy = 0.5f;
    [SerializeField] private float maxDirLineLength = 5;
    [Tooltip("Half the distance between the slingshot line's end points. (I.e., how much space should be " +
        "between the slingshot line's ends and the point they're centered on.")]
    [SerializeField] private float slingLineSeparation = 0.25f;

    private Vector3? flingStartPos;
    private Rigidbody bodyToFling;
    private Vector3 flingVector;

    void Update()
    {
        //A flingable body is selected and a starting position for the slingshot is stored
        if (bodyToFling && flingStartPos is Vector3 startPos)
        {
            //player holding slingshot input; hasn't flung the object yet
            if (slingshotInput.GetState(inputSource))
            {
                //--Save direction and strength of flingVector--
                Vector3 flingDir = Vector3.Normalize(startPos - handTransform.position);
                float pullDistance = Vector3.Distance(handTransform.position, startPos);
                pullDistance = Mathf.Clamp(pullDistance, 0, maxPullDistance);
                float percentStrength = Mathf.InverseLerp(0, maxPullDistance, pullDistance);

                flingVector = flingDir * Mathf.Lerp(0, maxForce, percentStrength);

                //--Update lines indicating what direction the object will be flung--
                //Put the end points of the slingshot off to the open and back of the hand. This just so happens to 
                //be the local up vector of the obj attach point
                Vector3 slingSeparationDir = handTransform.right;

                slingshotLine.enabled = true;
                slingshotLine.SetPosition(0, startPos - (slingSeparationDir * slingLineSeparation));
                slingshotLine.SetPosition(1, handTransform.position);
                slingshotLine.SetPosition(2, startPos - (slingSeparationDir * -slingLineSeparation));

                //Calculate the proper positions for the points on the directionLine.
                Vector3 startPoint = bodyToFling.transform.position;
                Vector3 endPoint = Vector3.Lerp(
                    startPoint,
                    flingDir * maxDirLineLength,
                    percentStrength
                );
                Vector3 arrowHeadPoint = Vector3.Lerp(startPoint, endPoint, 0.7f);

                //Finally, set the direction line's positions to the ones we calculated above.
                //Lines two and three are strange because they emulate an arrow with width.
                directionLine.enabled = true;
                directionLine.SetPosition(0, startPoint);
                directionLine.SetPosition(1, arrowHeadPoint);
                directionLine.SetPosition(2, arrowHeadPoint + arrowHeadPoint.normalized * 0.001f);
                directionLine.SetPosition(3, endPoint);
            }
            //player released slingshot input; should fling the object right now
            else
            {
                bodyToFling.AddForce(flingVector, ForceMode.Impulse);
                Debug.Log("Launched with force vector " + flingVector);

                //reset all variables related to slingshotting, because we just "used them up"
                bodyToFling = null;
                flingStartPos = null;
                flingVector = Vector3.zero;

                slingshotLine.enabled = false;
                directionLine.enabled = false;
            }
        }
        //Flingable body or starting pos not set, pointing at a flingable body
        else if (HandRaycastForFlingable(handTransform, out Rigidbody flingableBody))
        {
            targetLine.enabled = false;

            //player holding slingshot input
            if (slingshotInput.GetState(inputSource) && pointerInput.GetState(inputSource))
            {
                //Save the hand's current position and the flingable body it's pointing at
                flingStartPos = handTransform.position;
                bodyToFling = flingableBody;

                //Disable the pointer line renderer
                pointerLine.enabled = false;
            }
            //player not holding slingshot input
            else if (pointerInput.GetState(inputSource))
            {
                //draw a line to the flingable body the player is pointing at to indicate it's flingable
                Debug.Log("Pointing at flingable");
                pointerLine.enabled = true;
                pointerLine.SetPosition(0, handTransform.position);
                pointerLine.SetPosition(1, flingableBody.transform.position);
            }
        }
        //Flingable body or starting pos not set, and not pointing at a flingable body
        else
        {
            if (pointerInput.GetState(inputSource))
            {
                Vector3 startPoint = handTransform.position;
                Vector3 endPoint = handTransform.forward * maxGrabRange;
                Vector3 midPoint = Vector3.Lerp(startPoint, endPoint, 0.5f);

                targetLine.enabled = true;
                targetLine.SetPosition(0, startPoint);
                targetLine.SetPosition(1, midPoint);
                targetLine.SetPosition(2, endPoint);
            }
            else
            {
                targetLine.enabled = false;
            }
            //disable the pointer line renderer
            pointerLine.enabled = false;
        }
    }

    /// <summary>
    /// Cast a ray forward from hand and get the flingable rigidbody that was hit, if there is one.
    /// </summary>
    /// <param name="hand">The hand to cast from.</param>
    /// <param name="hitFlingableRB">The flingable rigidbody hit.</param>
    /// <returns>Whether a flingable rigidbody was hit or not.</returns>
    private bool HandRaycastForFlingable(Transform handTrans, out Rigidbody hitFlingableRB)
    {
        hitFlingableRB = null;

        //Cast a sphere forward from the given hand pos, and return the first hit.
        if (Physics.SphereCast(
            //Magic number the origin pos into the hand
            handTrans.position,
            targetAccuracy,
            //This is the only object in the hand that points in the intuitive forward direction
            handTrans.forward,
            out RaycastHit hit,
            maxGrabRange,
            ~LayerMask.GetMask("Player")
            ))
        {
            //If that hit has a rigidbody and is flingable, set hitFlingableRB to that rigidbody and return true.
            Rigidbody hitRB = hit.rigidbody;
            if (hitRB && hitRB.gameObject.CompareTag("Flingable"))
            {
                hitFlingableRB = hitRB;
                return true;
            }
        }

        //If we got this far, nothing we're looking for was hit. Return false.
        return false;
    }
}
