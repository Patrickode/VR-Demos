using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] private ParticleSystem damageParticles = null;
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private AudioSource audioSource = null;
    [SerializeField] private AudioClip damageClip = null;
    [SerializeField] private AudioClip dieClip = null;
    [SerializeField] private float maxHealth = 100.0f;
    [SerializeField] private float hitSoundPitchRange = 0.25f;

    private float currentHealth;
    private bool dying;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damageTaken)
    {
        damageParticles.Play();
        currentHealth -= damageTaken;
        if (!dying)
        {
            PlayWithRandomPitch(audioSource, damageClip, 1 - hitSoundPitchRange, 1 + hitSoundPitchRange, 0.25f);
        }
        else
        {
            PlayWithRandomPitch(audioSource, damageClip, 1, 1, 0.25f);
        }

        if (currentHealth <= 0 && currentHealth + damageTaken > 0)
        {
            rb.isKinematic = false;
            audioSource.pitch = 1;
            audioSource.PlayOneShot(dieClip, 0.6f);
            StartCoroutine(SetDying());
        }
    }

    private void PlayWithRandomPitch(AudioSource source, AudioClip clip, float rangeMin, float rangeMax,
        float volume = -1)
    {
        source.pitch = Random.Range(rangeMin, rangeMax);
        if (volume < 0) { source.PlayOneShot(clip); }
        else { source.PlayOneShot(clip, volume); }
    }

    private IEnumerator SetDying()
    {
        dying = true;
        yield return new WaitForSeconds(dieClip.length);
        dying = false;
    }
}
