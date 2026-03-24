using UnityEngine;

public abstract class GunSkillBase<T> : ProjectileSkillBase<T> where T : Projectile
{
    public override AbilityData Data => data;

    public override float ChargePercentage
    {
        get
        {
            if (!data.ChargeWithCooldown)
            {
                return 1f;
            }

            return base.ChargePercentage;
        }
    }

    private readonly GunSkillBaseData data;

    private float fireCooldownTimer = 0f;

    public GunSkillBase(AbilityController controller, GunSkillBaseData data, Stat cooldownTime) : base(data, controller, cooldownTime)
    {
        this.data = data;
    }

    public override bool CanActivate()
    {
        return fireCooldownTimer > data.FireCooldown;
    }

    protected override void Awake()
    {
        SetMaxCharges(data.Charges);
        SetCharges(data.Charges);
    }

    protected override void OnActivation()
    {
        fireCooldownTimer = 0f;
    }

    protected override void UpdateUnactive()
    {
        fireCooldownTimer += Time.deltaTime;
    }

    protected override void UpdateUnactiveBase()
    {
        if (data.ChargeWithCooldown)
        {
            base.UpdateUnactiveBase();
        }
    }
}