namespace Zeke.Abilities
{
    public class GetAbilityReference : GetAbilityStrategy
    {
        private readonly GetAbilityReferenceData data;

        public GetAbilityReference(GetAbilityReferenceData data)
        {
            this.data = data;
        }

        public override IAbility GetAbility(AbilityController controller)
        {
            if (controller.TryGetAbility(data.Reference.AbilityType, out IAbility ability))
            {
                if (ability.Data == data.Reference)
                {
                    return ability;
                }
            }

            return null;
        }
    }
}