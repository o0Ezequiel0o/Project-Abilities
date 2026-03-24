using UnityEngine;

public class SprintSkill : AbilityBase
{
    public override AbilityData Data => data;

    private bool hasRequiredComponents = true;

    private readonly SprintSkillData data;
    private readonly GameObject source;

    private readonly Stat effectDuration;
    private readonly Stat extraSpeed;

    private EntityMove entityMove;
    private float speedModifierApplied;

    public SprintSkill(GameObject source, AbilityController controller, SprintSkillData data, Stat cooldownTime, Stat effectDuration, Stat extraSpeed) : base(controller, cooldownTime, effectDuration)
    {
        this.source = source;
        this.data = data;

        this.extraSpeed = extraSpeed;
    }

    protected override void Awake()
    {
        LookForComponents();
    }

    public override bool CanActivate()
    {
        return !DurationActive && hasRequiredComponents;
    }

    public override bool CanDeactivate()
    {
        return DurationActive && hasRequiredComponents;
    }

    protected override void OnActivation()
    {
        speedModifierApplied = extraSpeed.Value;
        entityMove.MoveSpeed.ApplyFlatModifier(speedModifierApplied);
    }

    protected override void OnDeactivation()
    {
        entityMove.MoveSpeed.ApplyFlatModifier(-speedModifierApplied);
    }

    protected override bool CanUpgrade()
    {
        return !DurationActive;
    }

    protected override void UpgradeInternal()
    {
        UpgradeStats();
    }

    private void LookForComponents()
    {
        entityMove = source.GetComponentInChildren<EntityMove>();

        if (entityMove == null)
        {
            hasRequiredComponents = false;
        }
    }

    private void UpgradeStats()
    {
        effectDuration.Upgrade();
        extraSpeed.Upgrade();
    }
}