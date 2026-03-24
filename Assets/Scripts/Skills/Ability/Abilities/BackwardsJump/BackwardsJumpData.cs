using UnityEngine;

[CreateAssetMenu(fileName = "Backwards Jump", menuName = "ScriptableObjects/Abilities/BackwardsJump", order = 1)]
public class BackwardsJumpData : AbilityData
{
    [field: SerializeField] public float JumpForce { get; private set; }

    [field: Space]

    [field: SerializeField] public AbilityData AbilityToRestoreCharges { get; private set; }
    [SerializeField] private Stat chargesRestoreAmount;

    private Stat ChargesRestoreAmount => chargesRestoreAmount.DeepCopy();

    public override IAbility CreateAbility(GameObject source, AbilityController controller)
    {
        return new BackwardsJump(source, controller, this, CooldownTime, ChargesRestoreAmount);
    }
}