using UnityEngine;

namespace Zeke.Abilities.Modules
{
    public abstract class SummonType
    {
        public abstract SummonType DeepCopy();

        public abstract GameObject SpawnSummon(Vector3 position, Quaternion rotation, GameObject source);
    }
}