using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class MovementSpeed : AbilityModule
    {
        [SerializeField] private Stat flatValue;
        [SerializeField] private Stat multiplier;

        private EntityMove entityMove;
        private float speedModifierApplied;

        private bool hasRequiredComponents = true;

        private readonly Stat.Multiplier statMultiplier = new Stat.Multiplier();

        public MovementSpeed(MovementSpeed original)
        {
            flatValue = original.flatValue.DeepCopy();
            multiplier = original.multiplier.DeepCopy();
        }

        public override void OnInitialization(ModularAbilityController controller, Transform spawn, GameObject source, ModularAbility ability)
        {
            entityMove = source.GetComponent<EntityMove>();
            if (entityMove == null) hasRequiredComponents = false;
        }

        public override AbilityModule CreateDeepCopy() => new MovementSpeed(this);

        public override bool CanActivate() => true;

        public override bool CanDeactivate() => true;

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            if (!hasRequiredComponents) return;

            speedModifierApplied = flatValue.Value;
            entityMove.MoveSpeed.AddMultiplier(statMultiplier);
            entityMove.MoveSpeed.ApplyFlatModifier(speedModifierApplied);
        }

        public override void Deactivate()
        {
            if (!hasRequiredComponents) return;

            entityMove.MoveSpeed.RemoveMultiplier(statMultiplier);
            entityMove.MoveSpeed.ApplyFlatModifier(-speedModifierApplied);
        }

        public override void Upgrade()
        {
            flatValue.Upgrade();
            multiplier.Upgrade();

            statMultiplier.UpdateMultiplier(multiplier.Value);
        }
    }
}