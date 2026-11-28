using UnityEngine;

public class ShockStatus : StatusEffect
{
    public override StatusEffectData Data => effectData;
    private readonly ShockStatusData effectData;

    private readonly StatusEffectHandler statusEffectHandler;

    private readonly GameObject receiver;
    private readonly GameObject source;

    private readonly Stat.Multiplier moveMultiplier;
    private readonly Stat.Multiplier aimMultiplier;

    private EntityMove entityMove;
    private EntityAim entityAim;

    private float timer = 0f;

    public ShockStatus(StatusEffectHandler statusEffectHandler, GameObject receiver, GameObject source, ShockStatusData effectData, float moveSpeedMultiplier, float aimSpeedMultiplier)
    {
        this.statusEffectHandler = statusEffectHandler;

        this.receiver = receiver;
        this.source = source;

        this.effectData = effectData;

        moveMultiplier = new Stat.Multiplier(moveSpeedMultiplier);
        aimMultiplier = new Stat.Multiplier(aimSpeedMultiplier);
    }

    public override void Initialize()
    {
        if (receiver.TryGetComponent(out entityMove))
        {
            entityMove.MoveSpeed.AddMultiplier(moveMultiplier);
            entityMove.StopMoving();
        }
        if (receiver.TryGetComponent(out entityAim))
        {
            entityAim.RotationSpeed.AddMultiplier(aimMultiplier);
            entityAim.StopAiming();
        }
    }

    public override void OnStacksApplied(int stacks) {}

    public override void OnStacksRemoved(int stacks) {}

    public override void OnUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= effectData.Duration)
        {
           statusEffectHandler.RemoveEffect(this);
        }
    }

    public override void OnLateUpdate() {}
    
    public override void OnRemove()
    {
        if (entityMove != null || receiver.TryGetComponent(out entityMove))
        {
            entityMove.MoveSpeed.RemoveMultiplier(moveMultiplier);
        }
        if (entityAim != null || receiver.TryGetComponent(out entityAim))
        {
            entityAim.RotationSpeed.RemoveMultiplier(aimMultiplier);
        }
    }

    public override void OnDestroy() { }
}