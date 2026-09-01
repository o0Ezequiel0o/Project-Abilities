using UnityEngine;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules.Projectiles
{
    public class FireGiantOrb : FireDamageProjectile<GiantOrbProjectile>
    {
        private readonly FireGiantOrbData data;

        private readonly Stat smallOrbDamage;
        private readonly Stat smallOrbSpeed;
        private readonly Stat smallOrbRange;
        private readonly Stat smallOrbPierce;

        public FireGiantOrb(FireGiantOrbData data, Stat damage, Stat smallOrbDamage, Stat smallOrbSpeed, Stat smallOrbRange, Stat smallOrbPierce) : base(data, damage)
        {
            this.data = data;
            this.smallOrbDamage = smallOrbDamage;
            this.smallOrbSpeed = smallOrbSpeed;
            this.smallOrbRange = smallOrbRange;
            this.smallOrbPierce = smallOrbPierce;
        }

        public override bool CanLaunchProjectile() => true;

        public override void LaunchProjectile(Vector3 position, Vector3 direction, DamageData damageData, float knockback, float speed, float maxRange, GameObject source, Teams team)
        {
            GiantOrbProjectile projectile = projectilePool.Get(data.Prefab);
            projectile.Launch(position, speed, direction, maxRange, damageData, knockback, smallOrbDamage.Value, smallOrbSpeed.Value, smallOrbRange.Value, smallOrbPierce.ValueInt, source, team);
            projectile.gameObject.SetActive(true);
        }

        public override void Upgrade()
        {
            base.Upgrade();
            smallOrbDamage.Upgrade();
            smallOrbSpeed.Upgrade();
            smallOrbRange.Upgrade();
            smallOrbPierce.Upgrade();
        }
    }
}