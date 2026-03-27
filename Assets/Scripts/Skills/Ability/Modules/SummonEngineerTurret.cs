using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class SummonEngineerTurret : SummonType
    {
        [SerializeField] private EngineerTurret prefab;

        public SummonEngineerTurret() { }

        public SummonEngineerTurret(SummonEngineerTurret original)
        {
            prefab = original.prefab;
        }

        public override SummonType DeepCopy() => new SummonEngineerTurret(this);

        public override GameObject SpawnSummon(Vector3 position, Quaternion rotation, GameObject source)
        {
            EngineerTurret summon = GameObject.Instantiate(prefab, position, Quaternion.identity);
            summon.Initialize(source);
            return summon.gameObject;
        }
    }
}