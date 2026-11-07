using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class LockAbilityData : AbilityModuleData
    {
        [field: SerializeField] public AbilityType AbilityType { get; private set; }

        public override AbilityModule CreateModule()
        {
            return new LockAbility(this);
        }
    }
}