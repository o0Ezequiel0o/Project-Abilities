using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class Dash : AbilityModule
    {
        [SerializeField] private float jumpForce;
        [SerializeField] private Vector2 direction;

        private Physics physics;
        private EntityAim entityAim;

        private bool hasRequiredComponents = true;

        public Dash() { }

        public Dash(Dash original)
        {
            jumpForce = original.jumpForce;
            direction = original.direction;
        }

        public override AbilityModule DeepCopy() => new Dash(this);

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
            physics.AddForce(jumpForce, GetRelativeDirection(direction.normalized, entityAim.AimDirection));
        }

        public Vector2 GetRelativeDirection(Vector2 direction, Vector2 relativeTo)
        {
            float angle = Vector2.SignedAngle(Vector2.up, direction);
            return (Quaternion.Euler(0f, 0f, angle) * relativeTo).normalized;
        }
    }
}