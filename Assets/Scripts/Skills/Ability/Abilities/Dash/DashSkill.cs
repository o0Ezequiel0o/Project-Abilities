using UnityEngine;

public class DashSkill : AbilityBase
{
    public override AbilityData Data => data;

    private bool hasRequiredComponents = true;

    private readonly DashSkillData data;
    private readonly GameObject source;

    private Vector2 lastMoveDirection;

    private EntityMove entityMove;
    private Physics physics;

    private bool dashing = false;
    private float dashTimer = 0f;

    public DashSkill(GameObject source, AbilityController controller, DashSkillData data, Stat cooldownTime) : base(controller, cooldownTime)
    {
        this.source = source;
        this.data = data;
    }

    public override bool CanActivate()
    {
        return !DurationActive && hasRequiredComponents && !dashing;
    }

    public override bool CanDeactivate()
    {
        return DurationActive && hasRequiredComponents;
    }

    protected override void Awake()
    {
        LookForComponents();
    }

    public override void OnDestroy() { }

    private void LookForComponents()
    {
        if (!source.TryGetComponent(out physics)) return;
        if (!source.TryGetComponent(out entityMove)) return;

        hasRequiredComponents = true;
    }

    protected override void OnActivation()
    {
        Vector2 direction = entityMove.MoveDirection;

        if (direction == Vector2.zero)
        {
            direction = lastMoveDirection;
        }

        physics.AddForce(data.Force, direction);
    }

    protected override void OnDeactivation() { }

    protected override void UpdateAll()
    {
        if (!hasRequiredComponents) return;

        if (entityMove.MoveDirection != Vector2.zero)
        {
            lastMoveDirection = entityMove.MoveDirection;
        }

        if (!dashing) return;

        dashTimer += Time.deltaTime;

        if (dashTimer >= data.DashDuration)
        {
            dashing = false;
            dashTimer = 0f;
        }
    }

    protected override bool CanUpgrade()
    {
        return !DurationActive;
    }

    protected override void UpgradeInternal() { }
}