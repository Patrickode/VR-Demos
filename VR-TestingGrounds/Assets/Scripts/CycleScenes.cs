using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class CycleScenes : MonoBehaviour
{
    [SerializeField] private int minIndex = 0;
    [SerializeField] private int maxIndex = 5;
    [SerializeField] private SteamVR_Action_Boolean nextSceneInput = null;
    [SerializeField] private SteamVR_Action_Boolean lastSceneInput = null;

    private void Update()
    {
        if (nextSceneInput.GetStateDown(SteamVR_Input_Sources.Any))
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            SceneManager.LoadScene(nextSceneIndex <= maxIndex ? nextSceneIndex : minIndex);
        }
        else if (lastSceneInput.GetStateDown(SteamVR_Input_Sources.Any))
        {
            int lastSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;
            SceneManager.LoadScene(lastSceneIndex >= minIndex ? lastSceneIndex : maxIndex);
        }
    }
}
