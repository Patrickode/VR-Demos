using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmOnDetection : MonoBehaviour
{
    [SerializeField] private Light alarmLight = null;
    [SerializeField] private AudioSource alarmAudio = null;
    [Space(10)]
    [SerializeField] private float flashInterval = 0.5f;

    private Coroutine alarmCoroutine;

    private void Start()
    {
        DetectDisplaced.DisplacementDetected += SoundAlarm;
        DetectDisplaced.DisplacementAverted += CancelAlarm;
    }
    private void OnDestroy()
    {
        DetectDisplaced.DisplacementDetected -= SoundAlarm;
        DetectDisplaced.DisplacementAverted -= CancelAlarm;
    }

    private void SoundAlarm()
    {
        alarmAudio.Play();
        alarmCoroutine = StartCoroutine(AlarmFlash());
    }
    private void CancelAlarm()
    {
        alarmAudio.Stop();
        if (alarmCoroutine != null) { StopCoroutine(alarmCoroutine); }
    }

    private IEnumerator AlarmFlash()
    {
        alarmLight.enabled = true;
        float progress = 0;

        while (true)
        {
            progress += Time.deltaTime / flashInterval;

            if (progress >= 1)
            {
                progress--;
                alarmLight.enabled = !alarmLight.enabled;
            }
            yield return null;
        }
    }
}
