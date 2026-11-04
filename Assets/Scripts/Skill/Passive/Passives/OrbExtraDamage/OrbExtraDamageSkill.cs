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
        if (source.TryGetComponent(out DamageEventListener eventListener))
        {
            eventListener.OnDealtDamage.Subscribe(OnDealtDamage);
        }
    }

    public override void OnRemove()
    {
        if (source.TryGetComponent(out DamageEventListener eventListener))
        {
            eventListener.OnDealtDamage.Unsubscribe(OnDealtDamage);
        }
    }

    protected override void UpgradeInternal(int levels)
    {
        extraDamage.Upgrade(levels);
    }

    private void OnDealtDamage(Damageable.DamageEvent.OnDealtDamageEvent dealtDamageEvent)
    {
        Damageable.DamageEvent damageEvent = dealtDamageEvent.damageEvent;

        if (damageEvent.SourceObject == null) return;

        if (damageEvent.SourceObject.TryGetComponent(out OrbIdentifier _))
        {
            DamageInfo damageInfo = new DamageInfo(extraDamage.Value, data.ArmorPenetration, data.ProcCoefficient)
            {
                hit = false
            };

            damageEvent.Receiver.DealDamage(damageInfo, source, source);
        }
    }
}