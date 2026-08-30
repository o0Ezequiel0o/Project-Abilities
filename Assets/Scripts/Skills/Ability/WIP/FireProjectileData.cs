using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Projectiles
{
    [Serializable]
    public class FireProjectileData : AbilityModuleData
    {
        [field: Header("Casting")]
        [field: SerializeField] public float FireDistance { get; private set; }
        [field: SerializeField] public Limits Spread { get; private set; } = Limits.Zero;

        [field: Space]

        [SerializeReferenceDropdown, SerializeReference] private FireProjectileTypeData projectile;

        [SerializeField] private Stat speed;
        [SerializeField] private Stat maxRange;

        public override AbilityModule CreateModule()
        {
            return new FireProjectile(this, projectile.CreateProjectileTypeModule(), speed.DeepCopy(), maxRange.DeepCopy());
        }
    }
}