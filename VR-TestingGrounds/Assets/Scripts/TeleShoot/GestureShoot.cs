using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class GestureShoot : MonoBehaviour
{
    [Tooltip("The hand that will be doing the shooting.")]
    [SerializeField] private Hand shootHand = null;
    [Tooltip("The source to take input from. Should be the left or right hand, depending on what shootHand is.")]
    [SerializeField] private SteamVR_Input_Sources inputSource = SteamVR_Input_Sources.RightHand;
    [Tooltip("The Input/Action to use for shooting.")]
    [SerializeField] private SteamVR_Action_Boolean shootInput = null;
    [Tooltip("If shootHand's velocity's magnitude is smaller than this, don't shoot.")]
    [SerializeField] [Range(0.1f, 5.0f)] private float minGestureVelocity = 0.1f;
    [SerializeField] private float maxBulletVelocity = -1;
    [Tooltip("Multiplies the shot bullet's velocity by this value.")]
    [SerializeField] private float shootVelMultiplier = 2.0f;
    [SerializeField] private TeleBullet bulletPrefab = null;
    [SerializeField] private Transform bulletSpawnPosRef = null;
    [SerializeField] private Vector3 bulletSpawnOffset = Vector3.zero;
    [SerializeField] private ParticleSystem pSystem = null;

    private void Start()
    {
        if (maxBulletVelocity < minGestureVelocity) { maxBulletVelocity = Mathf.Infinity; }
    }

    private void Update()
    {
        Vector3 handVel = shootHand.GetTrackedObjectVelocity();

        if (shootInput.GetState(inputSource))
        {
            if (!pSystem.isPlaying) { pSystem.Play(); }
        }
        else if (shootInput.GetStateUp(inputSource))
        {
            pSystem.Stop();

            if (handVel.sqrMagnitude > minGestureVelocity * minGestureVelocity)
            {
                //Instantiate the bullet and make it move in the direction of the hand's velocity
                var instantiatedBullet = Instantiate(bulletPrefab, bulletSpawnPosRef);
                instantiatedBullet.InitBullet(
                    Vector3.ClampMagnitude(handVel * shootVelMultiplier, maxBulletVelocity),
                    bulletSpawnOffset,
                    transform
                );
            }
        }
    }
}
