using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class RechargeWithKillsData : RechargeTypeData
    {
        [field: SerializeField] public ValueType ValueType { get; private set; }
        [SerializeField] private Stat amount;

        public override RechargeType CreateRechargeTypeModule()
        {
            return new RechargeWithKills(this, amount.DeepCopy());
        }
    }
}