using System.Collections.Generic;
using UnityEngine;

namespace Zeke.Abilities.Modules.Summoning
{
    public class Summon : AbilityModule
    {
        private readonly SummonData data;

        private readonly List<SummonModule> modules;
        private readonly Stat maxSummons;

        private Transform spawn;
        private GameObject source; 

        private readonly List<GameObject> summons = new List<GameObject>();
        private readonly List<Collider2D> hits = new List<Collider2D>();

        private Vector3 WorldSpawnPosition => spawn.position + (data.SpawnDistance * spawn.up);

        public Summon(SummonData data, List<SummonModule> modules, Stat maxSummons)
        {
            this.data = data;
            this.modules = modules;
            this.maxSummons = maxSummons;
        }

        public override bool CanActivate() => !IsBlocked(WorldSpawnPosition, data.SpawnBlockRadius, data.SpawnBlockLayers);

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
            if (data.FixedRotation) rotation = Quaternion.identity;
            GameObject summonInstance = GameObject.Instantiate(data.Summon, position, rotation);

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