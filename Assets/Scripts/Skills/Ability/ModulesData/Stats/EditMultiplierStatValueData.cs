using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
    public class EditMultiplierStatValueData : AbilityModuleData
    {
        [field: SerializeField] public bool Permanent { get; private set; } = false;
        [SerializeField] private Stat amount;
        [SerializeReferenceDropdown, SerializeReference] private GetStatStrategyData stat;

        public override AbilityModule CreateModule()
        {
            return new EditMultiplierStatValue(this, stat.CreateStatStrategy(), amount.DeepCopy());
        }
    }
}