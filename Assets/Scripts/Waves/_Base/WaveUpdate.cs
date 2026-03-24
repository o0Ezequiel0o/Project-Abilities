using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave Update", menuName = "ScriptableObjects/WaveManager/WaveUpdate", order = 1)]
public class WaveUpdate : ScriptableObject
{
    [field: Header("Settings")]
    [field: SerializeField] public int Wave { get; private set; }
    [field: SerializeField] public bool BossWave { get; private set; }
    [field: Header("Spawning")]
    [field: SerializeField] public int PointsPerWave { get; private set; }
    [field: SerializeField] public float SpawnCooldown { get; private set; }
    [field: SerializeField] public List<WaveSpawnable> AddToPool { get; private set; }
    [field: SerializeField] public List<WaveSpawnable> RemoveFromPool { get; private set; }

    void OnValidate()
    {
        ClampValues();
    }

    void Reset()
    {
        ClampValues();
    }

    void ClampValues()
    {
        Wave = Mathf.Max(1, Wave);
        PointsPerWave = Mathf.Max(0, PointsPerWave);
    }
}