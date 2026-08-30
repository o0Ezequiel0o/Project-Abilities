using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public partial class RechargeWithDistance : RechargeType
    {
        private readonly RechargeWithDistanceData data;

        private readonly Stat amount;
        private readonly Stat distanceForAmount;

        private Vector3 lastPosition = Vector3.zero;

        public RechargeWithDistance(RechargeWithDistanceData data, Stat amount, Stat distanceForAmount)
        {
            this.data = data;
            this.amount = amount;
            this.distanceForAmount = distanceForAmount;
        }

        public override void OnInitialization(AbilityController controller, GameObject source, Ability ability)
        {
            base.OnInitialization(controller, source, ability);
            lastPosition = source.transform.position;
        }

        public override bool CanActivate() => true;
        public override bool CanUpgrade() => true;

        public override void UpdateDuration()
        {
            float distanceTravelled = Vector3.Distance(lastPosition, source.transform.position);
            float value = 0f;

            if (distanceTravelled != 0f && distanceForAmount.Value != 0f)
            {
                value = distanceTravelled / distanceForAmount.Value;
            }

            UpdateCooldown(value * amount.Value, data.ValueType);
            lastPosition = source.transform.position;
        }

        public override void Upgrade()
        {
            amount.Upgrade();
            distanceForAmount.Upgrade();
        }
    }
}