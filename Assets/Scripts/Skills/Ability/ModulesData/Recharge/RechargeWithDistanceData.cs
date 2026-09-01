using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class RechargeWithDistanceData : RechargeTypeData
    {
        [field: SerializeField] public ValueType ValueType { get; private set; }
        [SerializeField] private Stat amount;
        [SerializeField] private Stat distanceForAmount;

        public override RechargeType CreateRechargeTypeModule()
        {
            return new RechargeWithDistance(this, amount.DeepCopy(), distanceForAmount.DeepCopy());
        }
    }
}