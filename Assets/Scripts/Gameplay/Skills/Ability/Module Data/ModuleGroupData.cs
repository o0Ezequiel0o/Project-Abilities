using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class ModuleGroupData : AbilityModuleData
    {
        [SerializeReference, SerializeReferenceDropdown] private List<AbilityModuleData> modules;

        public override AbilityModule CreateModule()
        {
            List<AbilityModule> instanceModules = new List<AbilityModule>();

            for (int i = 0; i < modules.Count; i++)
            {
                instanceModules.Add(modules[i].CreateModule());
            }

            return new ModuleGroup(this, instanceModules);
        }
    }
}