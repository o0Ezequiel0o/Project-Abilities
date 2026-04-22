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

    public override void Initialize()
    {
        if (!receiver.TryGetComponent(out damageable) || effectData.Ticks <= 0)
        {
            statusEffectHandler.RemoveEffect(this);
        }
    }

    public override void OnStacksApplied(int stacks) { }

    public override void OnStacksRemoved(int stacks) { }

    public override void OnUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= effectData.TickTime)
        {
            float damage = damageable.MaxHealth.Value * effectData.MaxHealthRatio.GetValue(stacks);
            damage = Mathf.Min(damage, effectData.MaxDamage.GetValue(stacks));
            DamageInfo damageInfo = new DamageInfo(damage, 0f, 0f) { hit = false };
            damageable.DealDamage(damageInfo, source, null);
            SpawnBloodStain();
            UpdateTicks();
        }
    }

    public override void OnLateUpdate() { }
    
    public override void OnRemove() { }

    public override void OnDestroy() { }

    private void SpawnBloodStain()
    {
        GameObject newBloodStain = StatusEffectParticlesPool.Get(effectData.StainParticles);
        newBloodStain.transform.position = receiver.transform.position;
        newBloodStain.SetActive(true);
    }

    private void UpdateTicks()
    {
        currentTicks += 1;
        timer = 0f;

        if (currentTicks >= effectData.Ticks)
        {
            statusEffectHandler.RemoveEffect(this, 1);
            currentTicks = 0;
        }
    }
}