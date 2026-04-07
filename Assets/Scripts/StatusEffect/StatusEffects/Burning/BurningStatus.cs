using UnityEngine;

public class BurningStatus : StatusEffect
{
    public override StatusEffectData Data => effectData;
    private readonly BurningStatusData effectData;

    private readonly StatusEffectHandler statusEffectHandler;

    private readonly GameObject receiver;
    private readonly GameObject source;

    private Damageable damageable;

    private int currentTicks = 0;
    private float timer = 0f;

    private GameObject particles;

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

        particles = StatusEffectParticlesPool.Get(effectData.Particles);
        particles.transform.position = Vector3.zero;
        particles.SetActive(true);
    }

    public override void OnStackApply()
    {
        //FINISH BURNING IMPLEMENTATION
    }

    public override void OnUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= effectData.TickTime)
        {
            DamageInfo damageInfo = new(effectData.Damage, 0f, 0f) { hit = false };
            damageable.DealDamage(damageInfo, source, null);
            UpdateTicks();
        }
    }

    public override void OnLateUpdate()
    {
        if (particles == null) return;
        particles.transform.position = receiver.transform.position;
    }

    public override void OnRemove()
    {
        if (particles == null) return;
        particles.SetActive(false);
    }

    public override void OnDestroy()
    {
        if (particles == null) return;
        particles.SetActive(false);
    }

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