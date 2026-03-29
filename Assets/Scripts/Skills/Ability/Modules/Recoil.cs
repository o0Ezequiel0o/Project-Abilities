using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class Recoil : AbilityModule
    {
        [SerializeField] private Stat force;
        [SerializeField] private Vector2 direction;

        private Physics physics;
        private EntityAim entityAim;

        private bool hasRequiredComponents = true;

        public Recoil() { }

        public Recoil(Recoil original)
        {
            direction = original.direction;
            force = original.force.DeepCopy();
        }

        public override AbilityModule DeepCopy() => new Recoil(this);

        public override bool CanActivate() => true;
        public override bool CanUpgrade() => true;

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            if (!source.TryGetComponent(out entityAim)) hasRequiredComponents = false;
            if (!source.TryGetComponent(out physics)) hasRequiredComponents = false;
        }

        public override void Activate(bool holding)
        {
            if (!hasRequiredComponents) return;
            physics.AddForce(force.Value, GetRelativeDirection(direction.normalized, entityAim.AimDirection));
        }

        public override void Upgrade()
        {
            force.Upgrade();
        }

        public Vector2 GetRelativeDirection(Vector2 direction, Vector2 relativeTo)
        {
            float angle = Vector2.SignedAngle(Vector2.up, direction);
            return (Quaternion.Euler(0f, 0f, angle) * relativeTo).normalized;
        }
    }
}