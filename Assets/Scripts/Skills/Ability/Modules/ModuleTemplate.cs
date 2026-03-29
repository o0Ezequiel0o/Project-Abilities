using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class ModuleTemplate : AbilityModule
    {
        [SerializeField] private Stat value;

        public ModuleTemplate() { } //empty constructor for unity's inspector default parameters

        public ModuleTemplate(ModuleTemplate original) //constructor for deep copy
        {
            value = original.value.DeepCopy();
        }

        public override AbilityModule DeepCopy() => new ModuleTemplate(this);

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability) { }

        public override bool CanActivate() => true;
        public override bool CanUpgrade() => true;

        public override void Activate(bool holding) { }
        public override void Deactivate() { }

        public override void Update() { }

        public override void UpdateActive() { }
        public override void UpdateUnactive() { }

        public override void LateUpdate() { }

        public override void Upgrade()
        {
            base.Upgrade();
            value.Upgrade();
            //Remember to upgrade all stats
        }

        public override void Destroy() { }
    }
}