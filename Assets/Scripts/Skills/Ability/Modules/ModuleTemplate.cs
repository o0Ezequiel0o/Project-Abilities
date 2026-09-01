using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class ModuleTemplate : AbilityModule
    {
        private readonly ModuleTemplateData data;

        private readonly Stat value;

        public ModuleTemplate(ModuleTemplateData data, Stat value)
        {
            this.data = data;
            this.value = value;
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability) { }

        public override bool CanActivate() => true;
        public override bool CanUpgrade() => true;

        public override void Activate(bool holding) { }
        public override void Deactivate() { }

        public override void Update() { }

        public override void UpdateActive() { }
        public override void UpdateInactive() { }

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