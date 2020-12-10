using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TeleBullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rBody = null;
    /// <summary>The object that shot this bullet.</summary>
    private GameObject shooterObj = null;

    private void Start()
    {
        Destroy(gameObject, 5);
    }

    public void InitBullet(Vector3 velocity, Vector3 posOffset, Transform shooter = null)
    {
        //If shooter wasn't supplied, assume the shooter to be the parent of this object.
        shooterObj = shooter ? shooter.gameObject : transform.parent.gameObject;
        //offset from the parent by posOffset, then unparent.
        transform.localPosition += posOffset;
        transform.parent = null;

        rBody.velocity = velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //If this bullet collides with a damagable thing, teleport the shooter to that thing ater destroying it.
        if (collision.gameObject.CompareTag("Damageable"))
        {
            Vector3 otherPosition = collision.transform.position;
            float otherYScale = collision.transform.localScale.y;
            Destroy(collision.gameObject);

            if (shooterObj) { shooterObj.transform.position = otherPosition - Vector3.one * otherYScale; }
        }

        Destroy(gameObject);
    }
}
