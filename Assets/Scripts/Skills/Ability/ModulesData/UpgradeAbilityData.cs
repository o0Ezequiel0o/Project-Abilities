using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class UpgradeAbilityData : AbilityModuleData
    {
        [SerializeReference, SerializeReferenceDropdown] private GetAbilityStrategyData strategy = new GetAbilityTypeData();
        [field: SerializeField] public int Levels { get; private set; }

        public override AbilityModule CreateModule()
        {
            return new UpgradeAbility(this, strategy.CreateStrategy());
        }
    }
}