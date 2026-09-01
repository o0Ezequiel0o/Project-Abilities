using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class LockAbilityData : AbilityModuleData
    {
        [SerializeReference, SerializeReferenceDropdown] private GetAbilityStrategyData strategy = new GetAbilityTypeData();

        public override AbilityModule CreateModule()
        {
            return new LockAbility(this, strategy.CreateStrategy());
        }
    }
}