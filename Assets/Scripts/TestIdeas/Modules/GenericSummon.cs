using UnityEngine;

namespace Zeke.Abilities.Modules
{
    public class GenericSummon<T> : Summon where T : class
    {
        public GenericSummon(GenericSummon<T> original) : base(original) { }

        public override AbilityModule CreateDeepCopy() => new GenericSummon<T>(this);

        public override void Activate(bool holding) { }

        protected bool TrySpawnSummon(GameObject prefab, out T component)
        {
            component = null;

            Vector3 spawnPosition = spawn.position + (spawnDistance * spawn.up);
            if (IsBlocked(spawnPosition, spawnBlockRadius, spawnBlockLayers)) return false;

            GameObject spawnedSummon = SpawnSummon(prefab, spawnPosition);
            component = spawnedSummon.GetComponent<T>();

            return true;
        }
    }
}