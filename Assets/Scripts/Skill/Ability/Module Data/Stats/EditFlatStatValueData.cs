using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
    public class EditFlatStatValueData : AbilityModuleData
    {
        [field: SerializeField] public bool Permanent { get; private set; }  = false;
        [SerializeField] private Stat amount;
        [SerializeReferenceDropdown, SerializeReference] private GetStatStrategyData stat;

        public override AbilityModule CreateModule()
        {
            return new EditFlatStatValue(this, stat.CreateStatStrategy(), amount.DeepCopy());
        }
    }
}