using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Utils;

public class StatisticsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveCounter;
    [SerializeField] private TextMeshProUGUI timeAliveCounter;
    [SerializeField] private TextMeshProUGUI enemiesKilledCounter;
    [SerializeField] private TextMeshProUGUI hitsReceivedCounter;

    private WaveManager waveManager;

    private void Awake()
    {
        waveManager = GameManager.instance.GetWaveManager();
    }

    private void OnEnable()
    {
        waveManager.OnEnemyKilled += UpdateEnemiesKilledCounter;
        waveManager.OnHitReceived += UpdateHitsReceivedCounter;
        waveManager.OnTimeAliveChange += UpdateTimeAliveCounter;
        waveManager.OnWaveValueChange += UpdateWaveCounter;

        UpdateCounters();
    }

    private void OnDisable()
    {
        waveManager.OnEnemyKilled -= UpdateEnemiesKilledCounter;
        waveManager.OnHitReceived -= UpdateHitsReceivedCounter;
        waveManager.OnTimeAliveChange -= UpdateTimeAliveCounter;
        waveManager.OnWaveValueChange -= UpdateWaveCounter;
    }

    

    private void UpdateWaveCounter()
    {
        string currentWaveString = (waveManager.CurrentWave + 1).ToString();

        if (!waveCounter.text.Equals(currentWaveString))
            waveCounter.text = currentWaveString;
    }

    private void UpdateTimeAliveCounter()
    {
        string timeAliveString = Utils.FloatToTimeMillisecondsFormat(waveManager.TimeAlive);

        if (!timeAliveCounter.text.Equals(timeAliveString))
            timeAliveCounter.text = timeAliveString;
    }

    private void UpdateEnemiesKilledCounter()
    {
        string enemiesKilledString = waveManager.TotalEnemiesKilled.ToString();

        if (!enemiesKilledCounter.text.Equals(enemiesKilledString))
            enemiesKilledCounter.text = enemiesKilledString;
    }

    private void UpdateHitsReceivedCounter()
    {
        string hitsReceivedString = waveManager.HitsReceived.ToString();

        if (!hitsReceivedCounter.text.Equals(hitsReceivedString))
            hitsReceivedCounter.text = hitsReceivedString;
    }

    private void UpdateCounters()
    {
        UpdateWaveCounter();
        UpdateTimeAliveCounter();
        UpdateEnemiesKilledCounter();
        UpdateHitsReceivedCounter();
    }
}
