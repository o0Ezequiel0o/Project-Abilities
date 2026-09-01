using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class ModuleTemplateData : AbilityModuleData
    {
        [SerializeField] private Stat value;

        public override AbilityModule CreateModule()
        {
            return new ModuleTemplate(this, value.DeepCopy());
        }
    }
}