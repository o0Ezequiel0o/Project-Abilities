using System.Collections.Generic;
using UnityEngine;

public class SpinnerProjectileSkill : SpinnerBaseSkill<SpinnerProjectile>
{
    public override AbilityData Data => data;

    private readonly SpinnerProjectileSkillData data;
    private readonly GameObject source;

    private readonly Stat damage;

    public SpinnerProjectileSkill(GameObject source, AbilityController controller, SpinnerProjectileSkillData data, Stat cooldownTime, Stat duration, Stat distance, Stat speed, Stat amount, Stat damage) : base(source, controller, data, cooldownTime, duration, distance, speed, amount)
    {
        this.source = source;
        this.data = data;

        this.damage = damage;
    }

    public override bool CanActivate()
    {
        return !DurationActive;
    }

    public override bool CanDeactivate()
    {
        return DurationActive;
    }

    protected override void OnSpinnerInitialization(List<SpinnerProjectile> spawnedObjects)
    {
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            spawnedObjects[i].Launch(spawnedObjects[i].transform.position, 0f, Vector2.zero, Mathf.Infinity, damage.Value, source);
        }
    }

    protected override void OnActivation()
    {
        InitializeSpinner(distance.Value, speed.Value, Mathf.FloorToInt(amount.Value));
    }

    protected override void OnDeactivation()
    {
        spinnerInstance.DisablePivotChildren();
    }

    protected override void OnDurationFinished()
    {
        spinnerInstance.DisablePivotChildren();
    }

    protected override void UpdateActive()
    {
        base.UpdateActive();
    }

    protected override bool CanUpgrade()
    {
        return !DurationActive;
    }

    protected override void UpgradeInternal()
    {
        base.UpgradeInternal();
        damage.Upgrade();
    }
}