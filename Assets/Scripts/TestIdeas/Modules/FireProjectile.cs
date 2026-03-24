using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public abstract class FireProjectile : AbilityModule
    {
        [Header("Local Data")]
        [SerializeField] protected Stat speed;
        [SerializeField] protected Stat damage;
        [SerializeField] protected Stat maxRange;

        [Header("Should be Global Data")]
        [SerializeField] protected float fireDistance;
        [SerializeField] protected Limits spread;

        protected Transform spawn;
        protected GameObject source;
        protected ModularAbility ability;
        protected ModularAbilityController controller;

        public FireProjectile(FireProjectile original)
        {
            speed = original.speed.DeepCopy();
            damage = original.damage.DeepCopy();
            maxRange = original.maxRange.DeepCopy();

            fireDistance = original.fireDistance;
        }

        public override void OnInitialization(ModularAbilityController controller, Transform spawn, GameObject source, ModularAbility ability)
        {
            this.spawn = spawn;
            this.source = source;
            this.ability = ability;
            this.controller = controller;
        }

        public override bool CanActivate() => true;

        public override bool CanDeactivate() => true;

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            LaunchProjectile(spawn.position, spawn.up, source);
        }

        public override void Upgrade()
        {
            speed.Upgrade();
            damage.Upgrade();
            maxRange.Upgrade();
        }

        protected abstract void LaunchProjectile(Vector3 castWorldPosition, Vector3 castDirection, GameObject source);
    }
}