using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashInCollectables : MonoBehaviour
{
    [SerializeField] private AudioSource collectAudio = null;
    [SerializeField] private AudioClip collectAudioClip = null;

    public static Action<float> CollectableCollected;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectable") && other.TryGetComponent<Collectable>(out var collectable))
        {
            if (collectAudioClip) { collectAudio.PlayOneShot(collectAudioClip); }
            CollectableCollected?.Invoke(collectable.value);
            Destroy(collectable.gameObject);
        }
    }
}
