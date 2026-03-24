public struct DamageInfo
{
    public float baseDamage;
    public float procCoefficient;
    public float armorPenetration;

    public bool lethal;

    public DamageInfo(Damageable.DamageEvent damageEvent)
    {
        baseDamage = damageEvent.BaseDamage;
        procCoefficient = damageEvent.ProcCoefficient;
        armorPenetration = damageEvent.ArmorPenetration;

        lethal = damageEvent.IsLethal;
    }

    public DamageInfo(float baseDamage, float armorPenetration, float procCoefficient)
    {
        this.baseDamage = baseDamage;
        this.armorPenetration = armorPenetration;
        this.procCoefficient = procCoefficient;

        lethal = true;
    }

    public DamageInfo(float baseDamage, float armorPenetration, float procCoefficient, bool lethal)
    {
        this.baseDamage = baseDamage;
        this.armorPenetration = armorPenetration;
        this.procCoefficient = procCoefficient;

        this.lethal = lethal;
    }
}