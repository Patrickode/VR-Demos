using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDummy : MonoBehaviour
{
    [SerializeField] private AudioSource killAudioSource = null;
    [SerializeField] private AudioClip killClip = null;
    [SerializeField] private float killVolume = 0.75f;
    [Space(10)]
    [SerializeField] private float moveSpeed = 1.0f;
    [Tooltip("The distance this dummy must be from the target to consider the target reached.")]
    [SerializeField] private float targetDistance = 0.75f;
    private bool killed = false;
    private bool targetReached = false;

    public Transform Target { get; set; } = null;

    public static Action EnemyTargetReached;

    private void Start() { Killable.KillableKilled += OnKillableKilled; }
    private void OnDestroy() { Killable.KillableKilled -= OnKillableKilled; }

    private void OnKillableKilled(Killable killableKilled)
    {
        if (killableKilled.gameObject.Equals(gameObject))
        {
            killed = true;
        }
    }

    private void Update()
    {
        if (!killed && !targetReached)
        {
            Vector3 moveDir = Target.position - transform.position;
            moveDir.y = 0;
            moveDir.Normalize();

            transform.forward = moveDir;
            transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
            Debug.DrawRay(transform.position, moveDir, Color.cyan);

            if ((Target.position - transform.position).sqrMagnitude <= targetDistance * targetDistance)
            {
                targetReached = true;
                if (killClip) { killAudioSource.PlayOneShot(killClip, killVolume); }
                EnemyTargetReached?.Invoke();
            }
        }
    }
}
