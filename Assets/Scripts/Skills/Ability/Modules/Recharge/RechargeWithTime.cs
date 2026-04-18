using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public partial class RechargeWithTime : RechargeType
    {
        public RechargeWithTime() { }

        public override RechargeType DeepCopy() => new RechargeWithTime();

        public override bool CanActivate() => true;
        public override bool CanUpgrade() => true;

        public override void UpdateDuration()
        {
            UpdateCooldown(Time.deltaTime * CastSpeed.Value, ValueType.Flat);
        }
    }
}