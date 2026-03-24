using System.Collections.Generic;
using UnityEngine;

public abstract class SummonSkillBase<T> : AbilityBase where T : Component
{
    private readonly SummonSkillBaseData data;

    protected Vector3 WorldSpawnPosition => controller.CastWorldPosition + (data.SpawnDistance * controller.CastDirection);

    protected readonly List<GameObject> summons = new List<GameObject>();
    protected int MaxSummons => Mathf.FloorToInt(maxSummons.Value);

    private readonly Stat maxSummons;

    public SummonSkillBase(AbilityController controller, SummonSkillBaseData data, Stat cooldownTime, Stat maxSummons) : base(controller, cooldownTime)
    {
        this.data = data;
        this.maxSummons = maxSummons;
    }

    protected bool TrySpawnSummon(GameObject prefab, out T component)
    {
        component = null;

        Vector3 spawnPosition = controller.CastWorldPosition + (data.SpawnDistance * controller.CastDirection);
        if (IsBlocked(spawnPosition, data.SpawnBlockRadius, data.SpawnBlockLayers)) return false;

        GameObject spawnedSummon = SpawnSummon(prefab, spawnPosition);
        component = spawnedSummon.GetComponent<T>();

        return true;
    }

    protected bool IsBlocked(Vector3 position, float radius, LayerMask layers)
    {
        return Physics2D.OverlapCircle(position, radius, layers) != null;
    }

    protected bool TrySpawnSummon(GameObject prefab)
    {
        Vector3 spawnPosition = controller.CastWorldPosition + (data.SpawnDistance * controller.CastDirection);
        if (IsBlocked(spawnPosition, data.SpawnBlockRadius, data.SpawnBlockLayers)) return false;

        SpawnSummon(prefab, spawnPosition);
        return true;
    }

    protected void DestroySummon(GameObject summon)
    {
        GameObject.Destroy(summon);
        summons.Remove(summon);
    }

    protected void DestroyAllSummoned()
    {
        for (int i = 0; i < summons.Count; i++)
        {
            DestroySummon(summons[i]);
        }

        summons.Clear();
    }

    protected void UpgradeSummonStats()
    {
        maxSummons.Upgrade();
    }

    private GameObject SpawnSummon(GameObject prefab, Vector3 spawnPosition)
    {
        GameObject spawnedSummon = GameObject.Instantiate(prefab, spawnPosition, Quaternion.identity);

        if (!spawnedSummon.TryGetComponent(out TrackSummonDestruction trackSummonDestruction))
        {
            trackSummonDestruction = spawnedSummon.AddComponent<TrackSummonDestruction>();
        }

        trackSummonDestruction.onDestroy += OnSummonDestroyed;
        summons.Add(spawnedSummon);
        return spawnedSummon;
    }

    private void OnSummonDestroyed(GameObject summon)
    {
        summons.Remove(summon);
    }
}