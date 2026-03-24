using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class FireGiantOrb : GenericFireProjectile<GiantOrbProjectile>
    {
        [SerializeField] private Stat homingOrbDamage;
        [SerializeField] private Stat homingOrbSpeed;
        [SerializeField] private Stat homingOrbRange;

        public FireGiantOrb(FireGiantOrb original) : base(original)
        {
            homingOrbDamage = original.homingOrbDamage.DeepCopy();
            homingOrbSpeed = original.homingOrbSpeed.DeepCopy();
            homingOrbRange = original.homingOrbRange.DeepCopy();
        }

        public override AbilityModule DeepCopy() => new FireGiantOrb(this);

        public override void Activate(bool holding)
        {
            GiantOrbProjectile giantOrb = LaunchAndGetProjectile(spawn.position, spawn.up, source);
            giantOrb.SetHomingOrbsValues(homingOrbDamage.Value, homingOrbSpeed.Value, homingOrbRange.Value);
        }

        public override void Upgrade()
        {
            base.Upgrade();
            homingOrbDamage.Upgrade();
            homingOrbSpeed.Upgrade();
            homingOrbRange.Upgrade();
        }
    }
}