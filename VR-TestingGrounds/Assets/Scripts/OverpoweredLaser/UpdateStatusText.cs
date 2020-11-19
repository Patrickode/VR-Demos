using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateStatusText : MonoBehaviour
{
    [SerializeField] private TextMeshPro statusText = null;
    private int score;

    private void Start()
    {
        statusText.text = "Score: 0";

        Killable.KillableKilled += OnKillableKilled;
        EnemyDummy.EnemyTargetReached += OnEnemyTargetReached;
        InnocentDummy.InnocentKilled += OnInnocentKilled;
    }
    private void OnDestroy()
    {
        Killable.KillableKilled -= OnKillableKilled;
        EnemyDummy.EnemyTargetReached -= OnEnemyTargetReached;
        InnocentDummy.InnocentKilled -= OnInnocentKilled;
    }

    private void OnKillableKilled(Killable _)
    {
        score += 100;
        statusText.text = $"Score: {score}";
    }
    private void OnEnemyTargetReached()
    {
        Killable.KillableKilled -= OnKillableKilled;
        EnemyDummy.EnemyTargetReached -= OnEnemyTargetReached;
        InnocentDummy.InnocentKilled -= OnInnocentKilled;

        SetTextAndColor("Killed by enemy dummy!", Color.red);
    }
    private void OnInnocentKilled()
    {
        Killable.KillableKilled -= OnKillableKilled;
        EnemyDummy.EnemyTargetReached -= OnEnemyTargetReached;
        InnocentDummy.InnocentKilled -= OnInnocentKilled;

        score -= 100;
        SetTextAndColor("Killed an innocent dummy!", Color.red);
    }

    private void SetTextAndColor(string text, Color color)
    {
        statusText.text = text;
        statusText.color = color;
    }
}
