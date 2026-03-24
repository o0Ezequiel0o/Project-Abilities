using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class FireLightingBolt : GenericFireProjectile<LightingBoltProjectile>
    {
        [SerializeField] private Stat spreadTargets;

        public FireLightingBolt(FireLightingBolt original) : base(original)
        {
            original.spreadTargets = spreadTargets.DeepCopy();
        }

        public override AbilityModule CreateDeepCopy() => new FireLightingBolt(this);

        public override void Activate(bool holding)
        {
            LightingBoltProjectile lightingBolt = LaunchAndGetProjectile(spawn.position, spawn.up, source);
            lightingBolt.SetSpreadTargets(spreadTargets.Value);
        }

        public override void Upgrade()
        {
            base.Upgrade();
            spreadTargets.Upgrade();
        }
    }
}