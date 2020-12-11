using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;

public class DummyManager : MonoBehaviour
{
    [SerializeField] private Player player = null;
    [SerializeField] private EnemyDummy enemyDummyPrefab = null;
    [SerializeField] private InnocentDummy innocentDummyPrefab = null;
    [SerializeField] private ParticleSystem appearPSystem = null;
    [Space(10)]
    [SerializeField] private AudioSource loseSource = null;
    [SerializeField] private AudioClip loseClip = null;
    [SerializeField] private float loseVolume = 0.5f;
    [Space(10)]
    [Tooltip("X = minimum radius, Y = maximum radius")]
    [SerializeField] private Vector2 spawnRadiusRange = new Vector2(5, 8);
    [SerializeField] private float startSpawnDelay = 4;
    [SerializeField] private float spawnDelay = 2;
    [SerializeField] private float reloadDelay = 3;

    private Coroutine spawnCoroutine = null;

    private void Start()
    {
        //Swap sRR if x > y
        if (spawnRadiusRange.x > spawnRadiusRange.y)
        {
            spawnRadiusRange.Set(spawnRadiusRange.y, spawnRadiusRange.x);
        }
        StartCoroutine(SpawnDummies());

        EnemyDummy.EnemyTargetReached += StartReloadSceneDelayed;
        InnocentDummy.InnocentKilled += StartReloadSceneDelayed;
    }
    private void OnDestroy()
    {
        EnemyDummy.EnemyTargetReached -= StartReloadSceneDelayed;
        InnocentDummy.InnocentKilled -= StartReloadSceneDelayed;
    }

    private void StartReloadSceneDelayed()
    {
        EnemyDummy.EnemyTargetReached -= StartReloadSceneDelayed;
        InnocentDummy.InnocentKilled -= StartReloadSceneDelayed;

        if (spawnCoroutine != null) { StopCoroutine(spawnCoroutine); }
        StartCoroutine(ReloadSceneDelayed());
    }
    private IEnumerator ReloadSceneDelayed()
    {
        if (loseClip)
        {
            loseSource.Stop();
            loseSource.PlayOneShot(loseClip, loseVolume);
            yield return new WaitForSeconds(Mathf.Max(loseClip.length, reloadDelay)); 
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator SpawnDummies()
    {
        yield return new WaitForSeconds(startSpawnDelay);
        Vector2 lastRndOnUnitCircle = Vector2.one * Mathf.Infinity;

        while (true)
        {
            //Get a random point on the edge of a unit circle, try again if a zero vector is returned
            Vector2 rndOnUnitCircle = Vector2.zero;
            while (rndOnUnitCircle.sqrMagnitude < 0.95f
                || (lastRndOnUnitCircle - rndOnUnitCircle).sqrMagnitude < 0.5f)
            {
                rndOnUnitCircle = Random.insideUnitCircle.normalized;
            }
            lastRndOnUnitCircle = rndOnUnitCircle;

            //Make the random normalized point on a unit sphere be within the spawn radius range at random
            rndOnUnitCircle *= Random.Range(spawnRadiusRange.x, spawnRadiusRange.y);
            Vector3 spawnPos = new Vector3(rndOnUnitCircle.x, 0, rndOnUnitCircle.y);

            //2 in 3 chance of spawning an enemy dummy, otherwise, spawn innocent dummy
            if (Random.Range(1, 4) < 3)
            {
                spawnPos.y = enemyDummyPrefab.transform.localScale.y;

                var spawnedDummy = Instantiate(
                    enemyDummyPrefab,
                    spawnPos,
                    Quaternion.LookRotation(
                        Vector3.ProjectOnPlane(player.transform.position - spawnPos, Vector3.up)
                    )
                );
                spawnedDummy.Target = player.transform;
            }
            else
            {
                spawnPos.y = innocentDummyPrefab.transform.localScale.y;

                Instantiate(
                    innocentDummyPrefab,
                    spawnPos,
                    Quaternion.LookRotation(
                        Vector3.ProjectOnPlane(player.transform.position - spawnPos, Vector3.up)
                    )
                );
            }
            Instantiate(appearPSystem, spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
