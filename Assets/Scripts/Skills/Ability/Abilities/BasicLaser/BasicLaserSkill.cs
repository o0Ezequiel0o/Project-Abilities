using UnityEngine;

public class BasicLaserSkill : AbilityBase
{
    public override AbilityData Data => data;

    private readonly BasicLaserSkillData data;
    private readonly GameObject source;

    private readonly Stat damage;
    private readonly Stat damageCooldown;

    private readonly Stat maxRange;

    private Laser laserInstance;

    private bool laserEnabledThisFrame = false;

    public BasicLaserSkill(GameObject source, AbilityController controller, BasicLaserSkillData data, Stat cooldownTime, Stat damage, Stat damageCooldown, Stat maxRange) : base(controller, cooldownTime)
    {
        this.source = source;
        this.data = data;

        this.damage = damage;
        this.damageCooldown = damageCooldown;

        this.maxRange = maxRange;
    }

    public override bool CanActivate()
    {
        return laserInstance != null;
    }

    public override bool CanDeactivate()
    {
        return !laserEnabledThisFrame;
    }

    protected override void Awake()
    {
        GameObject laserGOInstance = GameObject.Instantiate(data.LaserPrefab, source.transform.position, Quaternion.identity);

        if (laserGOInstance.TryGetComponent(out laserInstance))
        {
            laserInstance.SetLaserValues(source, damage.Value, data.MaxPierce, damageCooldown.Value);
        }

        laserGOInstance.SetActive(false);
    }

    public override void OnDestroy()
    {
        if (laserInstance == null) return;
        GameObject.Destroy(laserInstance.gameObject);
    }

    protected override void OnActivation()
    {
        laserEnabledThisFrame = true;
    }

    protected override void OnDeactivation()
    {
        DisableLaser();
    }

    protected override void LateUpdateAll()
    {
        if (laserInstance == null) return;

        if (laserEnabledThisFrame)
        {
            laserInstance.UpdateLaser(controller.CastWorldPosition, controller.CastWorldRotation, controller.CastDirection, data.Radius, maxRange.Value);
            if (!laserInstance.gameObject.activeSelf) laserInstance.gameObject.SetActive(true);

            laserEnabledThisFrame = false;
        }
        else
        {
            DisableLaser();
        }
    }

    protected override bool CanUpgrade()
    {
        return true;
    }

    protected override void UpgradeInternal()
    {
        UpgradeStats();
    }

    private void DisableLaser()
    {
        if (laserInstance == null) return;

        if (laserInstance.gameObject.activeSelf)
        {
            laserInstance.gameObject.SetActive(false);
        }
    }

    private void UpgradeStats()
    {
        damage.Upgrade();
        maxRange.Upgrade();
        damageCooldown.Upgrade();

        if (laserInstance != null)
        {
            laserInstance.SetLaserValues(source, damage.Value, data.MaxPierce, damageCooldown.Value);
        }
    }
}