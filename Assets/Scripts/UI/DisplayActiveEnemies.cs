using System.Collections;
using UnityEngine;
using TMPro;

public class DisplayActiveEnemies : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private TextMeshProUGUI activeEnemiesText;

    [Header("Settings")]
    [SerializeField] private float updateInterval = 0.2f;

    private WaitForSeconds updateWaitSeconds;

    private int currentActiveSpawnablesAmount = 0;
    private int displayedActiveSpawnablesAmount = 0;

    void Awake()
    {
        updateWaitSeconds = new WaitForSeconds(updateInterval);
    }

    void Start()
    {
        if (waveManager != null || activeEnemiesText != null)
        {
            UpdateActiveEnemiesText();
            StartCoroutine(CheckForActiveEnemiesUpdate());
        }
    }

    void Update()
    {
        if (waveManager == null || activeEnemiesText == null) return;

        activeEnemiesText.text = waveManager.ActiveSpawnables.ToString();
    }

    IEnumerator CheckForActiveEnemiesUpdate()
    {
        while (true)
        {
            currentActiveSpawnablesAmount = waveManager.ActiveSpawnables;

            if (currentActiveSpawnablesAmount != displayedActiveSpawnablesAmount)
            {
                UpdateActiveEnemiesText();
            }

            yield return updateWaitSeconds;
        }
    }

    void UpdateActiveEnemiesText()
    {
        activeEnemiesText.text = waveManager.ActiveSpawnables.ToString();
        displayedActiveSpawnablesAmount = waveManager.ActiveSpawnables;
    }
}