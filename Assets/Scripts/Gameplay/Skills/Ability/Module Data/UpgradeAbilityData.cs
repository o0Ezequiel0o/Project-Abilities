using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class UpgradeAbilityData : AbilityModuleData
    {
        [field: SerializeField] public AbilityType AbilityType { get; private set; }
        [field: SerializeField] public int Levels { get; private set; }

        public override AbilityModule CreateModule()
        {
            return new UpgradeAbility(this);
        }
    }
}