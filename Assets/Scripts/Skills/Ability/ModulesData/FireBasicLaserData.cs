using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class FireBasicLaserData : AbilityModuleData
    {
        [field: SerializeField] public GameObject Prefab { get; private set; }

        [Space]

        [SerializeField] private Stat damage;
        [SerializeField] private Stat maxRange;

        [field: Space]

        [field: SerializeField] public float Radius { get; private set; }
        [field: SerializeField] public int Pierce { get; private set; }
        [SerializeField] private Stat damageCooldown;

        [field: Space]

        [field: SerializeField] public float ArmorPenetration { get; private set; } = 0f;
        [field: SerializeField] public float ProcCoefficient { get; private set; } = 1f;

        public override AbilityModule CreateModule()
        {
            return new FireBasicLaser(this, damage.DeepCopy(), maxRange.DeepCopy(), damageCooldown.DeepCopy());
        }
    }
}