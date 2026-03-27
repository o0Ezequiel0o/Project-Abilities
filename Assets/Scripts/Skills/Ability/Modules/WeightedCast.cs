using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class WeightedCast : AbilityModule
    {
        [SerializeField] private List<ModuleInfo> choices;

        private AbilityModule selectedModule;

        public WeightedCast() { }

        public WeightedCast(WeightedCast original)
        {
            choices = new List<ModuleInfo>();

            for (int i = 0; i < original.choices.Count; i++)
            {
                choices.Add(original.choices[i].DeepCopy());
            }
        }

        public override AbilityModule DeepCopy() => new WeightedCast(this);

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            for (int i = 0; i < choices.Count; i++)
            {
                choices[i].module?.OnInitialization(controller, spawn, source, ability);
            }
        }

        public override bool CanActivate() => true;
        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            selectedModule = WeightedSelect.SelectElement(choices).module;
            selectedModule?.Activate(holding);
        }

        public override void Deactivate()
        {
            selectedModule?.Deactivate();
            selectedModule = null;
        }

        public override void UpdateActive()
        {
            selectedModule?.UpdateActive();
        }

        public override void Update()
        {
            selectedModule?.Update();
        }

        public override void LateUpdate()
        {
            selectedModule?.LateUpdate();
        }

        public override void Upgrade()
        {
            for (int i = 0; i < choices.Count; i++)
            {
                choices[i].module?.Upgrade();
            }
        }

        public override void Destroy()
        {
            for (int i = 0; i < choices.Count; i++)
            {
                choices[i].module?.Destroy();
            }
        }

        [Serializable]
        private class ModuleInfo : IWeighted
        {
            [field: SerializeField] public int Weight { get; set; } = 1;
            [field: SerializeReferenceDropdown, SerializeReference] public AbilityModule module;

            public ModuleInfo() { }

            public ModuleInfo(ModuleInfo original)
            {
                Weight = original.Weight;
                module = original.module?.DeepCopy();
            }

            public ModuleInfo DeepCopy() => new ModuleInfo(this);
        }
    }
}