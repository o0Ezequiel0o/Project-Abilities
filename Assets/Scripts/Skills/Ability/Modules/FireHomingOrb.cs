using UnityEngine;
using System;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class FireHomingOrb : FireDamageProjectile<HomingOrbProjectile>
    {
        [SerializeField] private Stat pierce;

        [Header("Find Target")]
        [SerializeField] private float targetRange;
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private LayerMask blockLayer;

        public FireHomingOrb() { }

        public FireHomingOrb(FireHomingOrb original) : base(original)
        {
            targetRange = original.targetRange;
            targetLayer = original.targetLayer;
            blockLayer = original.blockLayer;
            pierce = original.pierce.DeepCopy();
        }

        public override FireProjectileType DeepCopy() => new FireHomingOrb(this);

        public override bool CanLaunchProjectile() => true;

        public override void LaunchProjectile(Vector3 position, Vector3 direction, float damage, float speed, float maxRange, GameObject source, Teams team)
        {
            Transform target = TargetAwareness.GetClosestTargetToDirection(position, direction, targetRange, targetLayer, blockLayer, target => TeamManager.IsEnemy(source, target));
            HomingOrbProjectile projectile = projectilePool.Get(prefab);
            projectile.Launch(position, speed, direction, maxRange, damage, pierce.ValueInt, target, source, team);
            projectile.gameObject.SetActive(true);
        }

        public override void Upgrade()
        {
            base.Upgrade();
            pierce.Upgrade();
        }
    }
}