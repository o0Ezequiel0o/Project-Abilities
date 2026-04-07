using System.Collections.Generic;
using UnityEngine;
using System;
using Zeke.PoolableGameObjects;

public class StatusEffectParticlesPool : Singleton<StatusEffectParticlesPool>
{
    [SerializeField] private List<PreparedEffects> preparedEffects;

    private readonly Dictionary<GameObject, GameObjectPool> particlePools = new Dictionary<GameObject, GameObjectPool>();

    public static GameObject Get(GameObject prefab)
    {
        return Get(prefab, null);
    }

    public static GameObject Get(GameObject prefab, Transform parent)
    {
        if (!Instance.particlePools.TryGetValue(prefab, out GameObjectPool pool))
        {
            Instance.particlePools.Add(prefab, new GameObjectPool());
        }

        return Instance.particlePools[prefab].Get(prefab, parent);
    }

    protected override void OnInitialization()
    {
        for (int i = 0; i < preparedEffects.Count; i++)
        {
            if (preparedEffects[i].prefab == null) continue;
            if (preparedEffects[i].amount <= 0) continue;

            particlePools.Add(preparedEffects[i].prefab, new GameObjectPool());

            for (int j = 0; j < preparedEffects[i].amount; j++)
            {
                GameObject goInstance = Instantiate(preparedEffects[i].prefab);
                particlePools[preparedEffects[i].prefab].Add(goInstance);
                goInstance.SetActive(false);
            }
        }

        preparedEffects.Clear();
    }

    private void OnDestroy()
    {
        Clear();
    }

    private void Clear()
    {
        foreach(GameObject key in particlePools.Keys)
        {
            particlePools[key].Clear();
        }

        particlePools.Clear();
    }

    [Serializable]
    private struct PreparedEffects
    {
        public GameObject prefab;
        public int amount;
    }
}