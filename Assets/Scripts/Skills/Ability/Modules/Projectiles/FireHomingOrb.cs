using UnityEngine;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules.Projectiles
{
    public class FireHomingOrb : FireDamageProjectile<HomingOrbProjectile>
    {
        private readonly FireHomingOrbData data;

        private readonly Stat pierce;

        public FireHomingOrb(FireHomingOrbData data, Stat damage, Stat pierce) : base(data, damage)
        {
            this.data = data;
            this.pierce = pierce;
        }

        public override bool CanLaunchProjectile() => true;

        public override void LaunchProjectile(Vector3 position, Vector3 direction, DamageData damageData, float knockback, float speed, float maxRange, GameObject source, Teams team)
        {
            Transform target = TargetAwareness.GetClosestTargetToDirection(position, direction, data.TargetRange, data.TargetLayer, data.BlockLayer,
                target => TeamManager.IsEnemy(source, target) && TargetAwareness.HasLineOfSight(position, target.transform.position, data.BlockLayer));
            HomingOrbProjectile projectile = projectilePool.Get(data.Prefab);
            projectile.Launch(position, speed, direction, maxRange, damageData, knockback, pierce.ValueInt, target, source, team);
            projectile.gameObject.SetActive(true);
        }

        public override void Upgrade()
        {
            base.Upgrade();
            pierce.Upgrade();
        }
    }
}