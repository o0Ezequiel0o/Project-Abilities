using UnityEngine;
using System;

namespace Zeke.Abilities
{
    [Serializable]
    public class GetAbilityType : GetAbilityStrategy
    {
        [SerializeField] private AbilityType type;

        public GetAbilityType() { }

        public GetAbilityType(GetAbilityType original)
        {
            type = original.type;
        }

        public override GetAbilityStrategy GetDeepCopy() => new GetAbilityType(this);

        public override IAbility GetAbility(AbilityController controller)
        {
            if (controller.TryGetAbility(type, out IAbility ability))
            {
                return ability;
            }

            return null;
        }
    }
}