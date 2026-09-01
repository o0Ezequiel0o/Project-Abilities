using System.Collections.Generic;
using UnityEngine;

namespace Zeke.Abilities.Modules
{
    public class WeightedCast : AbilityModule
    {
        private readonly WeightedCastData data;

        private readonly List<ModuleInfo> choices;

        private AbilityModule selectedModule;

        public WeightedCast(WeightedCastData data, List<ModuleInfo> choices)
        {
            this.data = data;
            this.choices = choices;
        }

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

        public readonly struct ModuleInfo : IWeighted
        {
            public readonly int Weight => Weight;

            public readonly int weight;
            public readonly AbilityModule module;

            public ModuleInfo(int weight, AbilityModule module)
            {
                this.weight = weight;
                this.module = module;
            }
        }
    }
}