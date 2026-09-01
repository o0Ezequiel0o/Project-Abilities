using UnityEngine;

namespace Zeke.Abilities.Modules
{
    public class CastOffset : AbilityModule
    {
        private readonly CastOffsetData data;

        private readonly AbilityModule module;

        private Transform newSpawn;

        public CastOffset(CastOffsetData data, AbilityModule module)
        {
            this.data = data;
            this.module = module;
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            newSpawn = new GameObject("castPosition").transform;
            newSpawn.parent = spawn.transform;

            newSpawn.SetLocalPositionAndRotation(data.Offset, Quaternion.Euler(0f, 0f, data.Angle));
            module?.OnInitialization(controller, newSpawn, source, ability);
        }

        public override void Activate(bool holding)
        {
            module?.Activate(holding);
        }

        public override void Deactivate()
        {
            module?.Deactivate();
        }

        public override bool CanActivate()
        {
            if (module == null) return true;
            return module.CanActivate();
        }

        public override bool CanUpgrade()
        {
            if (module == null) return true;
            return module.CanUpgrade();
        }

        public override void Update()
        {
            module?.Update();
        }

        public override void UpdateActive()
        {
            module?.UpdateActive();
        }

        public override void UpdateInactive()
        {
            module?.UpdateInactive();
        }

        public override void LateUpdate()
        {
            module?.LateUpdate();
        }

        public override void Upgrade()
        {
            module?.Upgrade();
        }

        public override void Destroy()
        {
            module?.Destroy();
            GameObject.Destroy(newSpawn.gameObject);
        }
    }
}