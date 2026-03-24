using UnityEngine;

public class UpgradePassiveSkill : AbilityBase
{
    public override AbilityData Data => data;

    private readonly UpgradePassiveSkillData data;
    private readonly GameObject source;

    private PassiveController passiveController;

    public UpgradePassiveSkill(GameObject source, AbilityController controller, UpgradePassiveSkillData data, Stat cooldownTime) : base(controller, cooldownTime)
    {
        this.source = source;
        this.data = data;
    }

    public override bool CanActivate()
    {
        return !DurationActive && passiveController != null;
    }

    public override bool CanDeactivate()
    {
        return DurationActive && passiveController != null;
    }

    protected override void Awake()
    {
        source.TryGetComponent(out passiveController);
    }

    protected override void OnActivation()
    {
        if (passiveController == null) return;

        for (int i = 0; i < data.UpgradeTimes; i++)
        {
            passiveController.UpgradePassive(data.Passive);
        }
    }

    protected override bool CanUpgrade()
    {
        return !DurationActive;
    }
}