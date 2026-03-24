using UnityEngine;
using System;

namespace Zeke.Abilities
{
    [Serializable]
    public class GetAbilityReference : GetAbilityStrategy
    {
        [SerializeField] private AbilityData reference;

        public GetAbilityReference() { }

        public GetAbilityReference(GetAbilityReference original)
        {
            reference = original.reference;
        }

        public override GetAbilityStrategy GetDeepCopy() => new GetAbilityReference(this);

        public override IAbility GetAbility(AbilityController controller)
        {
            if (controller.TryGetAbility(reference.AbilityType, out IAbility ability))
            {
                if (ability.Data == reference)
                {
                    return ability;
                }
            }

            return null;
        }
    }
}