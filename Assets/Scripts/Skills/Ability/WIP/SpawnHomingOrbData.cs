using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class SpawnHomingOrbsData : AbilityModuleData
    {
        [field: Header("Spawning")]
        [field: SerializeField] public HomingOrbProjectile Prefab { get; private set; }
        [field: SerializeField] public float Distance { get; private set; }
        [field: SerializeField] public float SpinSpeed { get; private set; }

        [Header("Homing Orbs")]
        [SerializeField] protected Stat amount;
        [SerializeField] protected Stat maxRange;
        [SerializeField] protected Stat pierce;
        [SerializeField] protected Stat damage;
        [SerializeField] protected Stat fireCooldown;

        [field: Space]

        [field: SerializeField] public float ArmorPenetration { get; private set; } = 0f;
        [field: SerializeField] public float ProcCoefficient { get; private set; } = 1f;
        [field: SerializeField] public float Knockback { get; private set; } = 1f;

        [Header("Targeting")]
        [field: SerializeField] public float DetectRadius { get; private set; }
        [field: SerializeField] public LayerMask HitLayers { get; private set; }
        [field: SerializeField] public LayerMask BlockLayers { get; private set; }

        [Header("Other")]
        [field: SerializeField] public float WarmUp { get; private set; }

        public override AbilityModule CreateModule()
        {
            return new SpawnHomingOrbs(this, amount.DeepCopy(), maxRange.DeepCopy(), pierce.DeepCopy(), damage.DeepCopy(), fireCooldown.DeepCopy());
        }
    }
}