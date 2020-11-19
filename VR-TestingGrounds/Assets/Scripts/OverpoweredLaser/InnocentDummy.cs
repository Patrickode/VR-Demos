using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnocentDummy : MonoBehaviour
{
    [SerializeField] private float timeUntilDisappear = 4;
    [SerializeField] private float disappearDuration = 1;
    private Coroutine disappearCoroutine = null;
    private Vector3 initialPosition;

    public static Action InnocentKilled;

    private void Start()
    {
        initialPosition = transform.position;
        disappearCoroutine = StartCoroutine(DisappearInTime());

        Killable.KillableKilled += OnKillableKilled;
    }
    private void OnDestroy() { Killable.KillableKilled -= OnKillableKilled; }

    private void OnKillableKilled(Killable killableKilled)
    {
        if (killableKilled.gameObject.Equals(gameObject))
        {
            StopCoroutine(disappearCoroutine);
            transform.position = initialPosition;

            InnocentKilled?.Invoke();
        }
    }

    private IEnumerator DisappearInTime()
    {
        yield return new WaitForSeconds(timeUntilDisappear);

        float progress = 0;
        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(startPos.x, startPos.y - (transform.localScale.y * 2), startPos.z);
        while (progress < 1)
        {
            progress += Time.deltaTime / disappearDuration;

            transform.position = new Vector3(
                startPos.x,
                Mathf.SmoothStep(startPos.y, endPos.y, progress),
                startPos.z
            );

            yield return null;
        }

        Destroy(gameObject);
    }
}
