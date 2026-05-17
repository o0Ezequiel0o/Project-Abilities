public readonly struct DamageData
{
    public readonly float armorPenetration;
    public readonly float procCoefficient;
    public readonly float damage;

    public DamageData(float damage, float armorPenetration, float procCoefficient)
    {
        this.damage = damage;
        this.armorPenetration = armorPenetration;
        this.procCoefficient = procCoefficient;
    }
}