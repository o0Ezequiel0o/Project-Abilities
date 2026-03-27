using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public partial class RechargeWithDistance : RechargeType
    {
        [SerializeField] private ValueType valueType;
        [SerializeField] private Stat amount;
        [SerializeField] private Stat distanceForAmount;

        private Vector3 lastPosition = Vector3.zero;

        public RechargeWithDistance() { }

        public RechargeWithDistance(RechargeWithDistance original)
        {
            valueType = original.valueType;
            amount = original.amount.DeepCopy();
            distanceForAmount = original.distanceForAmount.DeepCopy();
        }

        public override RechargeType DeepCopy() => new RechargeWithDistance(this);

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

            UpdateCooldown(value * amount.Value, valueType);
            lastPosition = source.transform.position;
        }

        public override void Upgrade()
        {
            amount.Upgrade();
            distanceForAmount.Upgrade();
        }
    }
}