using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class FireFireball : GenericFireProjectile<FireBallProjectile>
    {
        [SerializeField] private Stat damageRadius;

        public FireFireball(FireFireball original) : base(original)
        {
            damageRadius = original.damageRadius.DeepCopy();
        }

        public override AbilityModule DeepCopy() => new FireFireball(this);

        public override void Activate(bool holding)
        {
            Debug.Log(spawn);
            FireBallProjectile fireBall = LaunchAndGetProjectile(spawn.position, spawn.up, source);
            fireBall.SetDamageRadius(damageRadius.Value);
        }

        public override void Upgrade()
        {
            base.Upgrade();
            damageRadius.Upgrade();
        }
    }
}