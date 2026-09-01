using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class RechargeData : AbilityModuleData
    {
        [field: SerializeField] public Recharge.UpdateMode UpdateMode { get; private set; }
        [SerializeReferenceDropdown, SerializeReference] private RechargeTypeData type = new RechargeWithTimeData();

        public override AbilityModule CreateModule()
        {
            return new Recharge(this, type.CreateRechargeTypeModule());
        }
    }
}