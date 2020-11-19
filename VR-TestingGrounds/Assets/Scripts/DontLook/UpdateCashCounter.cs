using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateCashCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cashIndicator = null;
    [SerializeField] private float cashCounter = 0;

    private void Start()
    {
        cashIndicator.text = $"${cashCounter}";

        CashInCollectables.CollectableCollected += IncrementCashCounter;
    }
    private void OnDestroy()
    {
        CashInCollectables.CollectableCollected -= IncrementCashCounter;
    }

    private void IncrementCashCounter(float value)
    {
        cashCounter += value;
        cashIndicator.text = $"${cashCounter}";
    }
}
