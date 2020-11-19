using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHit : MonoBehaviour
{
    [Tooltip("When true, if the sword is held, half damage is dealt upon collision enter and exit, " +
        "instead of once on enter.")]
    [SerializeField] private bool doDoubleHit = false;
    [SerializeField] private float hitDamage = 2.0f;
    [SerializeField] private float hitCooldown = 0.1f;

    private bool isHeld = false;
    private bool onCooldown = false;
    private readonly Dictionary<Collider, Damageable> damagedCollisions =
        new Dictionary<Collider, Damageable>();
    //private List<Collision> collsOnCooldown;

    public void SetHeld(bool held) { isHeld = held; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Damageable")
            /*&& !collsOnCooldown.Contains(collision)*/)
        {
            Damageable damageableCollision = other.gameObject.GetComponent<Damageable>();

            if (isHeld && doDoubleHit)
            {
                damageableCollision.TakeDamage(hitDamage / 2);
                damagedCollisions.Add(other, damageableCollision);
            }
            else if (!onCooldown)
            {
                damageableCollision.TakeDamage(hitDamage);
                StartCoroutine(SetCooldown());
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (doDoubleHit && damagedCollisions.TryGetValue(other, out Damageable damagedCollision) && isHeld
            /*&& !collsOnCooldown.Contains(collision)*/)
        {
            damagedCollision.TakeDamage(hitDamage / 2);
            damagedCollisions.Remove(other);
            //StartCoroutine(SetCollCooldown(collision));
        }
    }

    private IEnumerator SetCooldown()
    {
        onCooldown = true;
        yield return new WaitForSeconds(hitCooldown);
        onCooldown = false;
    }

    //private IEnumerator SetCollCooldown(Collision collToCooldown)
    //{
    //    collsOnCooldown.Add(collToCooldown);
    //    yield return new WaitForSeconds(hitCooldown);
    //    collsOnCooldown.Remove(collToCooldown);
    //}
}
