using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class Dash : AbilityModule
    {
        private readonly DashData data;

        private Physics physics;
        private EntityAim entityAim;

        private bool hasRequiredComponents = true;

        public Dash(DashData data)
        {
            this.data = data;
        }

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
            physics.AddForce(data.JumpForce, GetRelativeDirection(data.Direction.normalized, entityAim.AimDirection));
        }

        public Vector2 GetRelativeDirection(Vector2 direction, Vector2 relativeTo)
        {
            float angle = Vector2.SignedAngle(Vector2.up, direction);
            return (Quaternion.Euler(0f, 0f, angle) * relativeTo).normalized;
        }
    }
}