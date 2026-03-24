using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class RecursiveCast : AbilityModule
    {
        [SerializeField] private List<ModuleSpawnData> modules;

        private GameObject source;
        private ModularAbility ability;
        private ModularAbilityController controller;

        public RecursiveCast(RecursiveCast original)
        {
            modules = new List<ModuleSpawnData>();

            for (int i = 0; i < original.modules.Count; i++)
            {
                modules.Add(original.modules[i].CreateDeepCopy());
            }
        }

        public override AbilityModule CreateDeepCopy() => new RecursiveCast(this);

        public override void OnInitialization(ModularAbilityController controller, Transform spawn, GameObject source, ModularAbility ability)
        {
            this.source = source;
            this.ability = ability;
            this.controller = controller;

            for (int i = 0; i < modules.Count; i++)
            {
                InitializeModules(modules[i]);
            }
        }

        private void InitializeModules(ModuleSpawnData moduleSpawnData)
        {
            Transform spawn = new GameObject("castPosition").transform;
            spawn.parent = source.transform;
            spawn.SetLocalPositionAndRotation(moduleSpawnData.offset, Quaternion.Euler(0f, 0f, moduleSpawnData.angle));

            for (int i = 0; i < moduleSpawnData.modules.Count; i++)
            {
                moduleSpawnData.modules[i].OnInitialization(controller, spawn, source, ability);
            }
        }

        public override void Activate(bool holding)
        {
            for (int i = 0; i < modules.Count; i++)
            {
                for (int x = 0; x < modules[i].modules.Count; x++)
                {
                    modules[i].modules[x].Activate(holding);
                }
            }
        }

        public override void Deactivate()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                for (int x = 0; x < modules[i].modules.Count; x++)
                {
                    modules[i].modules[x].Deactivate();
                }
            }
        }

        public override bool CanDeactivate()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                for (int x = 0; x < modules[i].modules.Count; x++)
                {
                    if (modules[i].modules[x].CanDeactivate())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override bool CanActivate()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                for (int x = 0; x < modules[i].modules.Count; x++)
                {
                    if (modules[i].modules[x].CanActivate())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override bool CanUpgrade()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                for (int x = 0; x < modules[i].modules.Count; x++)
                {
                    if (modules[i].modules[x].CanUpgrade())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override void OnDurationFinished()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                for (int x = 0; x < modules[i].modules.Count; x++)
                {
                    modules[i].modules[x].OnDurationFinished();
                }
            }
        }

        public override void Update()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                for (int x = 0; x < modules[i].modules.Count; x++)
                {
                    modules[i].modules[x].Update();
                }
            }
        }

        public override void UpdateActive()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                for (int x = 0; x < modules[i].modules.Count; x++)
                {
                    modules[i].modules[x].UpdateActive();
                }
            }
        }

        public override void UpdateUnactive()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                for (int x = 0; x < modules[i].modules.Count; x++)
                {
                    modules[i].modules[x].UpdateUnactive();
                }
            }
        }

        public override void LateUpdate()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                for (int x = 0; x < modules[i].modules.Count; x++)
                {
                    modules[i].modules[x].LateUpdate();
                }
            }
        }

        public override void Upgrade()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                for (int x = 0; x < modules[i].modules.Count; x++)
                {
                    modules[i].modules[x].Upgrade();
                }
            }
        }

        public override void Destroy()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                for (int x = 0; x < modules[i].modules.Count; x++)
                {
                    modules[i].modules[x].Destroy();
                }
            }
        }

        [Serializable]
        private class ModuleSpawnData
        {
            public Vector2 offset;
            public float angle;
            [SerializeReference, SerializeReferenceDropdown] public List<AbilityModule> modules;

            public ModuleSpawnData(Vector2 offset, float angle, List<AbilityModule> modules)
            {
                this.offset = offset;
                this.angle = angle;
                this.modules = modules;
            }

            public ModuleSpawnData CreateDeepCopy()
            {
                ModuleSpawnData moduleSpawnData = new ModuleSpawnData(offset, angle, new List<AbilityModule>());

                for (int i = 0; i < modules.Count; i++)
                {
                    moduleSpawnData.modules.Add(modules[i].CreateDeepCopy());
                }

                return moduleSpawnData;
            }
        }
    }
}