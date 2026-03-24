using UnityEngine;

public abstract class GunSkillBaseData : ProjectileSkillBaseData
{
    [field: Header("Gun Stats")]
    [field: SerializeField] public int Charges { get; private set; }
    [field: SerializeField] public float FireCooldown { get; private set; }

    [field: Space]

    [field: SerializeField] public bool ChargeWithCooldown { get; private set; }
}