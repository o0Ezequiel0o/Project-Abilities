using UnityEngine;

public class BurningStatus : StatusEffect
{
    public override StatusEffectData Data => effectData;
    private BurningStatusData effectData;

    private StatusEffectHandler statusEffectHandler;

    private GameObject receiver;
    private GameObject source;

    private Damageable damageable;

    private int currentTicks = 0;
    private float timer = 0f;

    private GameObject particleHandler;

    public BurningStatus(StatusEffectHandler statusEffectHandler, GameObject receiver, GameObject source, BurningStatusData effectData)
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

        particleHandler = Object.Instantiate(effectData.ParticleHandler, receiver.transform);
    }

    public override void OnStackApply() {}

    public override void OnUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= effectData.TickTime)
        {
            damageable.DealDamage(new DamageInfo(effectData.Damage, 0f, 0f), source, null);
            UpdateTicks();
        }
    }

    public override void OnRemove()
    {
        Object.Destroy(particleHandler);
    }

    void UpdateTicks()
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