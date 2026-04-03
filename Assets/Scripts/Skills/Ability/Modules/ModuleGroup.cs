using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class ModuleGroup : AbilityModule
    {
        [SerializeReference, SerializeReferenceDropdown] private List<AbilityModule> modules;

        public ModuleGroup() { }

        public ModuleGroup(ModuleGroup original)
        {
            modules = new List<AbilityModule>();

            for (int i = 0; i < original.modules.Count; i++)
            {
                modules.Add(original.modules[i].DeepCopy());
            }
        }

        public override AbilityModule DeepCopy() => new ModuleGroup(this);

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].OnInitialization(controller, spawn, source, ability);
            }
        }

        public override void Activate(bool holding)
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].Activate(holding);
            }
        }

        public override void Deactivate()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].Deactivate();
            }
        }

        public override bool CanActivate()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                if (modules[i].CanActivate())
                {
                    return true;
                }
            }

            return false;
        }

        public override bool CanUpgrade()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                if (modules[i].CanUpgrade())
                {
                    return true;
                }
            }

            return false;
        }

        public override void Update()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].Update();
            }
        }

        public override void UpdateActive()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].UpdateActive();
            }
        }

        public override void UpdateUnactive()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].UpdateUnactive();
            }
        }

        public override void LateUpdate()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].LateUpdate();
            }
        }

        public override void Upgrade()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].Upgrade();
            }
        }

        public override void Destroy()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].Destroy();
            }
        }
    }
}