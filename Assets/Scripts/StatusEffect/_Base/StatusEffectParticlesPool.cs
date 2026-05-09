using System.Collections.Generic;
using UnityEngine;
using System;
using Zeke.PoolableGameObjects;

public class StatusEffectParticlesPool : Singleton<StatusEffectParticlesPool>
{
    [SerializeField] private List<PreparedEffects> preparedEffects;

    private readonly Dictionary<GameObject, GameObjectPool<ParticleController>> particlePools = new Dictionary<GameObject, GameObjectPool<ParticleController>>();

    public static ParticleController Get(ParticleController controller)
    {
        return Get(controller, null);
    }

    public static ParticleController Get(ParticleController controller, Transform parent)
    {
        if (!Instance.particlePools.TryGetValue(controller.gameObject, out GameObjectPool<ParticleController> pool))
        {
            Instance.particlePools.Add(controller.gameObject, new GameObjectPool<ParticleController>());
        }

        return Instance.particlePools[controller.gameObject].Get(controller, parent);
    }

    protected override void OnInitialization()
    {
        for (int i = 0; i < preparedEffects.Count; i++)
        {
            if (preparedEffects[i].prefab == null) continue;
            if (preparedEffects[i].amount <= 0) continue;

            particlePools.Add(preparedEffects[i].prefab.gameObject, new GameObjectPool<ParticleController>());

            for (int j = 0; j < preparedEffects[i].amount; j++)
            {
                ParticleController controller = Instantiate(preparedEffects[i].prefab);
                particlePools[preparedEffects[i].prefab.gameObject].Add(controller);
                controller.gameObject.SetActive(false);
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
        public ParticleController prefab;
        public int amount;
    }
}