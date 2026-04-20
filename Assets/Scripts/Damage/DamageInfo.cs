using UnityEngine;

public struct DamageInfo
{
    public float baseDamage;
    public float procCoefficient;
    public float armorPenetration;

    public bool lethal;
    public bool hit;

    public Vector3 direction;

    public DamageInfo(Damageable.DamageEvent damageEvent)
    {
        baseDamage = damageEvent.BaseDamage;
        procCoefficient = damageEvent.ProcCoefficient;
        armorPenetration = damageEvent.ArmorPenetration;

        lethal = damageEvent.IsLethal;
        hit = damageEvent.IsHit;

        direction = damageEvent.Direction;
    }

    public DamageInfo(float baseDamage, float armorPenetration, float procCoefficient)
    {
        this.baseDamage = baseDamage;
        this.armorPenetration = armorPenetration;
        this.procCoefficient = procCoefficient;

        lethal = true;
        hit = true;

        direction = Vector3.zero;
    }

    public static DamageInfo Zero
    {
        get
        {
            return new DamageInfo(0f, 0f, 0f)
            {
                lethal = false,
                hit = false,
            };
        }
    }
}