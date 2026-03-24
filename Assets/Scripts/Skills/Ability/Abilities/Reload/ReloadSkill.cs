using UnityEngine;

public class ReloadSkill : AbilityBase
{
    public override AbilityData Data => data;

    private readonly ReloadSkillData data;

    private readonly Stat rechargeAmount;

    public ReloadSkill(GameObject _, AbilityController controller, ReloadSkillData data, Stat cooldownTime, Stat rechargeAmount, Stat rechargeTime) : base(controller, cooldownTime, rechargeTime)
    {
        this.data = data;
        this.rechargeAmount = rechargeAmount;
    }

    public override bool CanActivate()
    {
        return !DurationActive;
    }

    public override bool CanDeactivate()
    {
        return DurationActive;
    }

    protected override void OnActivation() { }

    protected override void OnDeactivation() { }

    protected override void OnDurationFinished()
    {
        if (controller.TryGetAbility(data.AbilityToRecharge, out IAbility ability))
        {
            ability.SetCharges(ability.Charges + Mathf.FloorToInt(rechargeAmount.Value));
        }
    }

    protected override bool CanUpgrade()
    {
        return !DurationActive;
    }

    protected override void UpgradeInternal()
    {
        UpgradeStats();
    }

    private void UpgradeStats()
    {
        rechargeAmount.Upgrade();
    }
}