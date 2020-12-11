using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnPunch : MonoBehaviour
{
    [SerializeField] private int explodeLayer = 10;
    [SerializeField] private ParticleSystem explodeSystem = null;
    [SerializeField] private AudioSource audioSource = null;
    [SerializeField] private AudioClip boomClip = null;
    [SerializeField] private AudioClip truthClip = null;

    private bool exploding;

    private void OnCollisionEnter(Collision collision)
    {
        if (!exploding && collision.gameObject.layer == explodeLayer)
        {
            Explode();
        }
    }

    public void Explode()
    {
        exploding = true;
        explodeSystem.gameObject.transform.parent = null;
        explodeSystem.Play();

        if (boomClip) { audioSource.PlayOneShot(boomClip, 0.6f); }
        if (truthClip) { audioSource.PlayOneShot(truthClip, 0.5f); }

        gameObject.GetComponent<Renderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;
        StartCoroutine(DestroyAfterPlay());
    }

    private IEnumerator DestroyAfterPlay()
    {
        yield return new WaitUntil(() => !audioSource.isPlaying);
        Destroy(gameObject);
    }
}
