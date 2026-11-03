using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class WeightedCastData : AbilityModuleData
    {
        [SerializeField] private List<ModuleInfo> choices;

        public override AbilityModule CreateModule()
        {
            List<WeightedCast.ModuleInfo> instanceModuleChoices = new List<WeightedCast.ModuleInfo>();

            for (int i = 0; i < choices.Count; i++)
            {
                instanceModuleChoices.Add(new WeightedCast.ModuleInfo(choices[i].Weight, choices[i].Module.CreateModule()));
            }

            return new WeightedCast(this, instanceModuleChoices);
        }

        [Serializable]
        private class ModuleInfo
        {
            [field: SerializeField, Min(1)] public int Weight { get; private set; } = 1;
            [field: SerializeReferenceDropdown, SerializeReference] public AbilityModuleData Module { get; private set; }
        }
    }
}