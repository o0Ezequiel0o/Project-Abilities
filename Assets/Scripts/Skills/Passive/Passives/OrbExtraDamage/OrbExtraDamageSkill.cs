using UnityEngine;

public class OrbExtraDamageSkill : PassiveBase
{
    public override PassiveData Data => data;
    private readonly OrbExtraDamageSkillData data;

    private readonly GameObject source;

    private readonly Stat extraDamage;

    public OrbExtraDamageSkill(GameObject source, PassiveController passiveController, OrbExtraDamageSkillData data, Stat extraDamage) : base(passiveController)
    {
        this.source = source;
        this.data = data;

        this.extraDamage = extraDamage;
    }

    public override void Awake()
    {
        Damageable.DamageEvent.onDamageDealt.Subscribe(source, OnDamageDealt);
    }

    public override void OnRemove()
    {
        Damageable.DamageEvent.onDamageDealt.Unsubscribe(source, OnDamageDealt);
    }

    public override void UpgradeInternal()
    {
        extraDamage.Upgrade();
    }

    private void OnDamageDealt(Damageable.DamageEvent damageEvent)
    {
        if (damageEvent.SourceObject == null) return;

        if (damageEvent.SourceObject.TryGetComponent(out OrbIdentifier _))
        {
            damageEvent.Receiver.DealDamage(new DamageInfo(extraDamage.Value, data.ArmorPenetration, data.ProcCoefficient), source, source);
        }
    }
}