using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Summoning
{
    [Serializable]
    public class Summon : AbilityModule
    {
        [Header("Summon")]
        [SerializeField] private GameObject summon;
        [SerializeReferenceDropdown, SerializeReference] private List<SummonModule> modules = new List<SummonModule>() { new JoinSourceTeam() };

        [Header("Spawning")]
        [SerializeField] private Stat maxSummons;

        [Space]

        [SerializeField] private bool fixedRotation;
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
            summon = original.summon;
            fixedRotation = original.fixedRotation;
            spawnDistance = original.spawnDistance;
            spawnBlockRadius = original.spawnBlockRadius;
            spawnBlockLayers = original.spawnBlockLayers;

            maxSummons = original.maxSummons.DeepCopy();

            modules = new List<SummonModule>();

            for (int i = 0; i < original.modules.Count; i++)
            {
                modules.Add(original.modules[i].DeepCopy());
            }
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
            for (int i = 0; i < summons.Count; i++)
            {
                for (int x = 0; x < modules.Count; x++)
                {
                    modules[x].OnDestroy(summons[i], source);
                }
            }

            DestroySummons();
        }

        private void SpawnSummon(Vector3 position, Quaternion rotation)
        {
            if (fixedRotation) rotation = Quaternion.identity;
            GameObject summonInstance = GameObject.Instantiate(summon, position, rotation);

            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].OnSummonSpawn(summonInstance, source);
            }

            TrackSummonInstanceDestruction(summonInstance);
            summons.Add(summonInstance);
        }

        private void TrackSummonInstanceDestruction(GameObject summonInstance)
        {
            if (!summonInstance.TryGetComponent(out DestroyEventTracker trackSummonDestruction))
            {
                trackSummonDestruction = summonInstance.AddComponent<DestroyEventTracker>();
            }

            trackSummonDestruction.onDestroy += OnSummonDestroyed;
        }

        protected bool IsBlocked(Vector3 position, float radius, LayerMask layers)
        {
            hits.Clear();

            ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = layers, useLayerMask = true };
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