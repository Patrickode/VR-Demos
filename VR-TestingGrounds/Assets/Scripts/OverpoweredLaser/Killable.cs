using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killable : MonoBehaviour
{
    [SerializeField] private ParticleSystem damageParticles = null;
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private Collider coll = null;
    [SerializeField] private AudioSource audioSource = null;
    [SerializeField] private AudioClip hitClip = null;
    [SerializeField] private AudioClip dieClip = null;
    [SerializeField] private float hitSoundPitchRange = 0.25f;

    private bool killed = false;

    public static Action<Killable> KillableKilled;

    private void OnCollisionEnter(Collision collision)
    {
        if (!killed && collision.gameObject.CompareTag("Killer"))
        {
            Die();
        }
    }

    private void Die()
    {
        killed = true;
        KillableKilled?.Invoke(this);
        rb.useGravity = true;
        coll.enabled = false;
        //Set the object's layer to player so it doesn't collide with the laser anymore
        gameObject.layer = 12;

        damageParticles.Play();

        audioSource.pitch = UnityEngine.Random.Range(1 - hitSoundPitchRange, 1 + hitSoundPitchRange);
        audioSource.PlayOneShot(hitClip, 0.25f);
        audioSource.PlayOneShot(dieClip, 0.6f);

        DestroyAfterMaxTime(damageParticles.main.duration, hitClip.length, dieClip.length);
    }

    private IEnumerator DestroyAfterMaxTime(params float[] times)
    {
        float timeToWait = 0;
        foreach (float time in times)
        {
            timeToWait = Mathf.Max(timeToWait, time);
        }

        yield return new WaitForSeconds(timeToWait);
        Destroy(gameObject);
    }
}
