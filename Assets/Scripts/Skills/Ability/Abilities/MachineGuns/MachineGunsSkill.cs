using System.Collections.Generic;
using UnityEngine;

public class MachineGunsSkill : ProjectileSkillBase<Projectile>
{
    public override AbilityData Data => data;

    private readonly bool hasRequiredComponents = true;

    private readonly MachineGunsSkillData data;
    private readonly GameObject source;

    private readonly List<GameObject> machineGuns = new List<GameObject>();

    public MachineGunsSkill(GameObject source, AbilityController controller, MachineGunsSkillData data, Stat cooldownTime) : base(data, controller, cooldownTime)
    {
        this.source = source;
        this.data = data;
    }

    protected override void Awake()
    {
        Vector3 offset = new Vector3(data.DistanceFromCenter, 0f);
        machineGuns.Add(GameObject.Instantiate(data.MachineGunsPrefab, source.transform));
        machineGuns.Add(GameObject.Instantiate(data.MachineGunsPrefab, source.transform));

        machineGuns[0].transform.localPosition = offset;
        machineGuns[1].transform.localPosition = -offset;
    }

    public override bool CanActivate()
    {
        return !DurationActive || hasRequiredComponents;
    }

    public override bool CanDeactivate()
    {
        return DurationActive || hasRequiredComponents;
    }

    protected override void OnActivation()
    {
        for (int i = 0; i < machineGuns.Count; i++)
        {
            float angleOffset = data.AngleOffset * Mathf.Sign(machineGuns[i].transform.localPosition.x);
            float newRotation = machineGuns[i].transform.eulerAngles.z + angleOffset;
            Vector3 direction = Quaternion.Euler(0f, 0f, newRotation) * Vector2.up;

            LaunchProjectile(machineGuns[i].transform.position, direction, source);
        }
    }

    protected override void OnDeactivation() { }

    protected override void UpdateActive()
    {
        TryDeactivate();
    }

    protected override bool CanUpgrade()
    {
        return !DurationActive;
    }

    protected override void UpgradeInternal()
    {
        UpgradeProjectileStats();
    }
}