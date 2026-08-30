using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Projectiles
{
    [Serializable]
    public abstract class FireDamageProjectileData<T> : FireProjectileTypeData where T : DamageProjectileBase
    {
        [field: SerializeField] public T Prefab { get; private set; }

        [field: Space]

        [field: SerializeField] protected Stat Damage { get; private set; }
        [field: SerializeField] public float ArmorPenetration { get; private set; } = 0f;
        [field: SerializeField] public float ProcCoefficient { get; private set; } = 1f;
        [field: SerializeField] public float Knockback { get; private set; } = 1f;
    }
}