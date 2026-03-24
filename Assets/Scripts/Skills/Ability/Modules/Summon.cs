using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class Summon : AbilityModule
    {
        [Header("Summon")]
        [SerializeField] protected GameObject prefab;
        [SerializeField] protected Stat maxSummons;

        [Header("Spawning")]
        [SerializeField] protected float spawnBlockRadius;
        [SerializeField] protected float spawnDistance;
        [SerializeField] protected LayerMask spawnBlockLayers;

        protected Transform spawn;
        protected GameObject source;
        protected Ability ability;
        protected AbilityController controller;

        protected readonly List<GameObject> summons = new List<GameObject>();

        protected Vector3 WorldSpawnPosition => spawn.position + (spawnDistance * spawn.up);

        public Summon(Summon original)
        {
            prefab = original.prefab;
            spawnDistance = original.spawnDistance;
            spawnBlockRadius = original.spawnBlockRadius;
            spawnBlockLayers = original.spawnBlockLayers;

            maxSummons = original.maxSummons.DeepCopy();
        }

        public override AbilityModule DeepCopy() => new Summon(this);

        public override bool CanActivate() => !IsBlocked(WorldSpawnPosition, spawnBlockRadius, spawnBlockLayers);

        public override bool CanUpgrade() => true;

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.spawn = spawn;
            this.source = source;
            this.ability = ability;
            this.controller = controller;
        }

        public override void Activate(bool holding)
        {
            TrySpawnSummon(prefab);
        }

        public override void Upgrade()
        {
            maxSummons.Upgrade();
        }

        protected bool TrySpawnSummon(GameObject prefab)
        {
            if (summons.Count >= maxSummons.ValueInt) return false;

            Vector3 spawnPosition = spawn.position + (spawnDistance * spawn.up);
            if (IsBlocked(spawnPosition, spawnBlockRadius, spawnBlockLayers)) return false;

            SpawnSummon(prefab, spawnPosition);
            return true;
        }

        protected GameObject SpawnSummon(GameObject prefab, Vector3 spawnPosition)
        {
            if (summons.Count >= maxSummons.ValueInt) return null;

            GameObject spawnedSummon = GameObject.Instantiate(prefab, spawnPosition, Quaternion.identity);

            if (!spawnedSummon.TryGetComponent(out TrackSummonDestruction trackSummonDestruction))
            {
                trackSummonDestruction = spawnedSummon.AddComponent<TrackSummonDestruction>();
            }

            trackSummonDestruction.onDestroy += OnSummonDestroyed;
            summons.Add(spawnedSummon);
            return spawnedSummon;
        }

        protected bool IsBlocked(Vector3 position, float radius, LayerMask layers)
        {
            return Physics2D.OverlapCircle(position, radius, layers) != null;
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

        private void OnSummonDestroyed(GameObject summon)
        {
            summons.Remove(summon);
        }
    }
}