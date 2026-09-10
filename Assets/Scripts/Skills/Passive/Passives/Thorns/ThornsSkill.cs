using UnityEngine;
using static Damageable;

public class ThornsSkill : PassiveBase
{
    public override PassiveData Data => data;
    private readonly ThornsSkillData data;

    private readonly GameObject source;

    private readonly Stat damagePerArmor;

    public ThornsSkill(GameObject source, PassiveController passiveController, ThornsSkillData data, Stat damagePerArmor) : base(passiveController)
    {
        this.source = source;
        this.data = data;

        this.damagePerArmor = damagePerArmor;
    }

    public override void Awake()
    {
        if (source.TryGetComponent(out Damageable damageable))
        {
            damageable.onTakenDamage.Subscribe(OnTakenDamage);
        }
    }

    public override void OnRemove()
    {
        if (source.TryGetComponent(out Damageable damageable))
        {
            damageable.onTakenDamage.Unsubscribe(OnTakenDamage);
        }
    }

    protected override void UpgradeInternal()
    {
        damagePerArmor.Upgrade();
    }

    private void OnTakenDamage(DamageEvent damageEvent)
    {
        if (damageEvent.SourceUser == null || damageEvent.SourceUser == source) return;

        if ((damageEvent.SourceUser.transform.position - source.transform.position).sqrMagnitude > data.MaxRange * data.MaxRange) return;

        float damage = damageEvent.Receiver.Armor.Value * damagePerArmor.Value * CalculateDamageReduction(damageEvent.Receiver.Armor.Value);

        if (damageEvent.SourceUser.TryGetComponent(out Damageable damageable))
        {
            DamageInfo damageInfo = new DamageInfo(damage, data.ArmorPenetration, data.ProcCoefficient)
            {
                direction = (damageEvent.SourceUser.transform.position - source.transform.position).normalized
            };

            damageable.DealDamage(damageInfo, source, source);
        }
    }
}