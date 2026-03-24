using UnityEngine;

public class BackwardsJump : AbilityBase
{
    public override AbilityData Data => data;

    private bool hasRequiredComponents = true;

    private readonly BackwardsJumpData data;
    private readonly GameObject source;

    private Physics physics;
    private EntityAim entityAim;

    private readonly Stat chargesRestoreAmount;

    public BackwardsJump(GameObject source, AbilityController controller, BackwardsJumpData data, Stat cooldownTime, Stat chargesRestoreAmount) : base(controller, cooldownTime)
    {
        this.source = source;
        this.data = data;

        this.chargesRestoreAmount = chargesRestoreAmount;
    }

    public override bool CanActivate()
    {
        return !DurationActive;
    }

    public override bool CanDeactivate()
    {
        return DurationActive;
    }

    protected override void Awake()
    {
        LookForComponents();
    }

    private void LookForComponents()
    {
        if (!source.TryGetComponent(out entityAim)) hasRequiredComponents = false;
        if (!source.TryGetComponent(out physics)) hasRequiredComponents = false;
    }

    protected override void OnActivation()
    {
        if (!hasRequiredComponents) return;
        physics.AddForce(data.JumpForce, -entityAim.AimDirection);

        if (controller.TryGetAbility(data.AbilityToRestoreCharges.AbilityType, out IAbility ability))
        {
            if (ability.Data == data.AbilityToRestoreCharges)
            {
                ability.SetCharges(ability.Charges + Mathf.FloorToInt(chargesRestoreAmount.Value));
            }
        }
    }

    protected override void UpdateActive()
    {
        TryDeactivate();
    }

    protected override bool CanUpgrade()
    {
        return !DurationActive;
    }

    protected override void UpgradeInternal()
    {
        chargesRestoreAmount.Upgrade();
    }
}