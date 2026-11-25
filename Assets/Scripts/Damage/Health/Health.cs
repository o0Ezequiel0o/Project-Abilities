using UnityEngine;

[CreateAssetMenu(fileName = "Health", menuName = "Health Types/Health", order = 0)]
public class Health : HealthTypeData
{
    public override HealthType CreateHealthType(Stat max, Stat regen, Stat receivedMultiplier)
    {
        return new HealthType().Initialize(this, max, regen, receivedMultiplier);
    }

    //Add check for if this one is full when doing shield regen in Damageable
    public override float UpdateRegeneration(HealthType health, Damageable damageable, float regenTimer)
    {
        float newValue = 0f;

        if (health.Value < health.Max.Value)
        {
            newValue = health.Regen.Value * regenTimer;
        }

        damageable.onAnyHealthUpdate?.Invoke();

        return newValue;
    }
}