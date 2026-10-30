using UnityEngine;
using TMPro;

public class EntityTracker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayText;

    private SpawnableSpawner spawner;

    private void Start()
    {
        spawner = FindAnyObjectByType<SpawnableSpawner>();
    }

    private void Update()
    {
        displayText.SetText(spawner.AliveSpawnables.ToString());
    }
}