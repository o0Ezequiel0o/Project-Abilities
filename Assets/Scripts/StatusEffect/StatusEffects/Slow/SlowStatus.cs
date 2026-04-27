using UnityEngine;

public class SlowStatus : StatusEffect
{
    public override StatusEffectData Data => effectData;
    private readonly SlowStatusData effectData;

    private readonly StatusEffectHandler statusEffectHandler;

    private readonly GameObject receiver;
    private readonly GameObject source;

    private readonly Stat.Multiplier moveSpeedMultiplier;
    private EntityMove entityMove;

    private float timer = 0f;

    public SlowStatus(StatusEffectHandler statusEffectHandler, GameObject receiver, GameObject source, SlowStatusData effectData)
    {
        this.statusEffectHandler = statusEffectHandler;

        this.receiver = receiver;
        this.source = source;

        this.effectData = effectData;
        moveSpeedMultiplier = new Stat.Multiplier(1f);
    }

    public override void Initialize()
    {
        if (receiver.TryGetComponent(out entityMove))
        {
            entityMove.MoveSpeed.AddMultiplier(moveSpeedMultiplier);
        }
        else
        {
            statusEffectHandler.RemoveEffect(this);
        }
    }

    public override void OnStacksApplied(int amount)
    {
        moveSpeedMultiplier.UpdateMultiplier(effectData.MoveSpeedMultiplier.GetValue(stacks));
    }

    public override void OnStacksRemoved(int amount)
    {
        moveSpeedMultiplier.UpdateMultiplier(effectData.MoveSpeedMultiplier.GetValue(stacks));
    }

    public override void OnUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= effectData.Duration)
        {
            statusEffectHandler.RemoveEffect(this, 1);
            timer = 0f;
        }
    }

    public override void OnLateUpdate() {}
    
    public override void OnRemove()
    {
        if (entityMove == null) return;
        entityMove.MoveSpeed.RemoveMultiplier(moveSpeedMultiplier);
    }

    public override void OnDestroy() {}
}