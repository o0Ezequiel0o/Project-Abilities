using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class Summon : AbilityModule
    {
        [Header("Summon")]
        [SerializeReferenceDropdown, SerializeReference] private SummonType summon;
        [SerializeField] private Stat maxSummons;

        [Header("Spawning")]
        [SerializeField] private float spawnBlockRadius;
        [SerializeField] private float spawnDistance;
        [SerializeField] private LayerMask spawnBlockLayers;

        private Transform spawn;
        private GameObject source; 

        private readonly List<GameObject> summons = new List<GameObject>();
        private readonly List<Collider2D> hits = new List<Collider2D>();

        private Vector3 WorldSpawnPosition => spawn.position + (spawnDistance * spawn.up);

        public Summon() { }

        public Summon(Summon original)
        {
            spawnDistance = original.spawnDistance;
            spawnBlockRadius = original.spawnBlockRadius;
            spawnBlockLayers = original.spawnBlockLayers;

            summon = original.summon.DeepCopy();
            maxSummons = original.maxSummons.DeepCopy();
        }

        public override AbilityModule DeepCopy() => new Summon(this);

        public override bool CanActivate() => !IsBlocked(WorldSpawnPosition, spawnBlockRadius, spawnBlockLayers);

        public override bool CanUpgrade() => true;

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.spawn = spawn;
            this.source = source;
        }

        public override void Activate(bool holding)
        {
            if (summons.Count >= maxSummons.ValueInt)
            {
                DestroySummon(summons[0]);
            }

            SpawnSummon(WorldSpawnPosition, spawn.rotation);
        }

        public override void Upgrade()
        {
            maxSummons.Upgrade();
        }

        public override void Destroy()
        {
            DestroySummons();
        }

        private void SpawnSummon(Vector3 position, Quaternion rotation)
        {
            GameObject summonInstance = summon.SpawnSummon(position, rotation, source);
            TrackSummonInstanceDestruction(summonInstance);
            summons.Add(summonInstance);
        }

        private void TrackSummonInstanceDestruction(GameObject summonInstance)
        {
            if (!summonInstance.TryGetComponent(out TrackSummonDestruction trackSummonDestruction))
            {
                trackSummonDestruction = summonInstance.AddComponent<TrackSummonDestruction>();
            }

            trackSummonDestruction.onDestroy += OnSummonDestroyed;
        }

        protected bool IsBlocked(Vector3 position, float radius, LayerMask layers)
        {
            hits.Clear();

            ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = layers };
            Physics2D.OverlapCircle(position, radius, contactFilter, hits);

            for (int i = 0; i < hits.Count; i++)
            {
                if (hits[i].gameObject != source)
                {
                    return true;
                }
            }

            return false;
        }

        protected void DestroySummon(GameObject summon)
        {
            summons.Remove(summon);
            GameObject.Destroy(summon);
        }

        protected void DestroySummons()
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