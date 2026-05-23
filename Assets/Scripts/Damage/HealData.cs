public readonly struct HealData
{
    public readonly float procCoefficient;
    public readonly float healing;

    public HealData(float healing, float procCoefficient)
    {
        this.healing = healing;
        this.procCoefficient = procCoefficient;
    }
}