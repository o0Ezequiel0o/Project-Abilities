using UnityEngine;

public abstract class SummonSkillBaseData : AbilityData
{
    [Header("Summon")]
    [field: SerializeField] public GameObject SummonPrefab { get; private set; }

    [Header("Spawning")]
    [field: SerializeField] public float SpawnBlockRadius { get; private set; }
    [field: SerializeField] public float SpawnDistance { get; private set; }
    [field: SerializeField] public LayerMask SpawnBlockLayers { get; private set; }

    [Header("Settings")]
    [SerializeField] private Stat maxSummons;

    protected Stat MaxSummons => maxSummons.DeepCopy();
}
