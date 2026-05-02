using UnityEngine;

public class AmbushSkill : PassiveBase
{
    public override PassiveData Data => data;
    private readonly AmbushSkillData data;

    private readonly GameObject source;

    private StatusEffectHandler statusEffectHandler;

    private bool ready = false;
    private float timer = 0f;

    private readonly Stat damageMultiplier;

    public AmbushSkill(GameObject source, PassiveController passiveController, AmbushSkillData data, Stat damageMultiplier) : base(passiveController)
    {
        this.source = source;
        this.data = data;

        this.damageMultiplier = damageMultiplier;
    }

    public override void Awake()
    {
        Damageable.DamageEvent.onDealDamage.Subscribe(source, OnDealDamage);

        statusEffectHandler = source.GetComponent<StatusEffectHandler>();
    }

    public override void Update()
    {
        if (ready) return;

        timer += Time.deltaTime;

        if (timer >= data.TimeToActivate)
        {
            ready = true;
            ShowVisual();
        }
    }

    public override void OnRemove()
    {
        Damageable.DamageEvent.onDealDamage.Unsubscribe(source, OnDealDamage);

        if (statusEffectHandler != null)
        {
            statusEffectHandler.RemoveEffect(data.ActiveIndicator);
        }
    }

    protected override void UpgradeInternal()
    {
        damageMultiplier.Upgrade();
    }

    private void OnDealDamage(Damageable.DamageEvent damageEvent)
    {
        if (damageEvent.SourceObject == null) return;

        if (damageEvent.SourceObject.TryGetComponent(out SniperProjectileIdentifier _))
        {
            if (ready)
            {
                damageEvent.Multiplier.Multiply(damageMultiplier.Value);

                ready = false;
                HideVisual();
            }

            timer = 0f;
        }
    }

    private void ShowVisual()
    {
        if (statusEffectHandler == null) return;
        statusEffectHandler.ApplyEffect(data.ActiveIndicator, source);
    }

    private void HideVisual()
    {
        if (statusEffectHandler == null) return;
        statusEffectHandler.RemoveEffect(data.ActiveIndicator);
    }
}