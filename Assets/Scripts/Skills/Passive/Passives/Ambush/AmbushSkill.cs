using UnityEngine;

public class AmbushSkill : PassiveBase
{
    public override PassiveData Data => data;
    private readonly AmbushSkillData data;

    private readonly GameObject source;

    private float timer = 0f;

    public AmbushSkill(GameObject source, PassiveController passiveController, AmbushSkillData data) : base(passiveController)
    {
        this.source = source;
        this.data = data;
    }

    public override void Awake()
    {
        Damageable.DamageEvent.onDamageDealt.Subscribe(source, OnDamageDealt);
        Damageable.DamageEvent.onDealDamage.Subscribe(source, OnDealDamage);
    }

    public override void Update()
    {
        timer += Time.deltaTime;
    }

    public override void OnRemove()
    {
        Damageable.DamageEvent.onDamageDealt.Unsubscribe(source, OnDamageDealt);
        Damageable.DamageEvent.onDealDamage.Unsubscribe(source, OnDealDamage);
    }

    private void OnDamageDealt(Damageable.DamageEvent _)
    {
        timer = 0f;
    }

    private void OnDealDamage(Damageable.DamageEvent damageEvent)
    {
        if (timer >= data.TimeToActivate)
        {
            damageEvent.Multiplier.Multiply(data.DamageMultiplier);
        }

        timer = 0f;
    }
}