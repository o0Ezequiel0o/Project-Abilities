using UnityEngine;
using System;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class FireGiantOrb : FireDamageProjectile<GiantOrbProjectile>
    {
        [SerializeField] private Stat smallOrbDamage;
        [SerializeField] private Stat smallOrbSpeed;
        [SerializeField] private Stat smallOrbRange;
        [SerializeField] private Stat smallOrbPierce;

        public FireGiantOrb() { }

        public FireGiantOrb(FireGiantOrb original) : base(original)
        {
            smallOrbDamage = original.smallOrbDamage.DeepCopy();
            smallOrbSpeed = original.smallOrbSpeed.DeepCopy();
            smallOrbRange = original.smallOrbRange.DeepCopy();
            smallOrbPierce = original.smallOrbPierce.DeepCopy();
        }

        public override FireProjectileType DeepCopy() => new FireGiantOrb(this);

        public override bool CanLaunchProjectile() => true;

        public override void LaunchProjectile(Vector3 position, Vector3 direction, float damage, float speed, float maxRange, GameObject source, Teams team)
        {
            GiantOrbProjectile projectile = projectilePool.Get(prefab);
            projectile.Launch(position, speed, direction, maxRange, damage, smallOrbDamage.Value, smallOrbSpeed.Value, smallOrbRange.Value, smallOrbPierce.ValueInt, source, team);
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