using UnityEngine;

public class BleedingStatus : StatusEffect
{
    public override StatusEffectData Data => effectData;
    private readonly BleedingStatusData effectData;

    private readonly StatusEffectHandler statusEffectHandler;

    private Damageable damageable;

    private readonly GameObject receiver;
    private readonly GameObject source;

    private float timer = 0f;
    private int currentTicks;

    public BleedingStatus(StatusEffectHandler statusEffectHandler, GameObject receiver, GameObject source, BleedingStatusData effectData)
    {
        this.statusEffectHandler = statusEffectHandler;

        this.receiver = receiver;
        this.source = source;

        this.effectData = effectData;
    }

    public override void OnApply()
    {
        if (!receiver.TryGetComponent(out damageable) || effectData.Ticks <= 0)
        {
            statusEffectHandler.RemoveEffect(this);
        }
    }

    public override void OnStackApply() {}

    public override void OnUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= effectData.TickTime)
        {
            float damage = damageable.MaxHealth.Value * effectData.DamageHealthRatio.CalculateValue(stacks);
            damage = Mathf.Min(damage, effectData.MaxDamage.CalculateValue(stacks));
            damageable.DealDamage(new DamageInfo(damage, 0f, 0f), source, null);
            UpdateTicks();
        }
    }
    
    public override void OnRemove() {}

    private void UpdateTicks()
    {
        currentTicks += 1;
        timer = 0f;

        if (currentTicks >= effectData.Ticks)
        {
            statusEffectHandler.RemoveOneEffectStack(this);
            currentTicks = 0;
        }
    }
}