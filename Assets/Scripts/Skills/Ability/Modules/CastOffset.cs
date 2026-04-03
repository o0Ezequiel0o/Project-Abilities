using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class CastOffset : AbilityModule
    {
        [SerializeField] private Vector2 offset;
        [SerializeField] private float angle;
        [SerializeReference, SerializeReferenceDropdown] public AbilityModule module;

        public CastOffset() { }

        public CastOffset(CastOffset original)
        {
            offset = original.offset;
            angle = original.angle;

            module = original.module?.DeepCopy();
        }

        public override AbilityModule DeepCopy() => new CastOffset(this);

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            Transform newSpawn = new GameObject("castPosition").transform;
            newSpawn.parent = spawn.transform;

            newSpawn.SetLocalPositionAndRotation(offset, Quaternion.Euler(0f, 0f, angle));
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

        public override void UpdateUnactive()
        {
            module?.UpdateUnactive();
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
        }
    }
}