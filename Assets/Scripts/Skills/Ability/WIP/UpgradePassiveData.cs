using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class UpgradePassiveData : AbilityModuleData
    {
        [field: SerializeField] public PassiveData Passive { get; private set; }
        [field: SerializeField] public int Levels { get; private set; }

        public override AbilityModule CreateModule()
        {
            return new UpgradePassive(this);
        }
    }
}