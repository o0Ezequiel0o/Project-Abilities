using UnityEngine;

public class SpawnEngineerTurretSkill : SummonSkillBase<EngineerTurret>
{
    public override AbilityData Data => data;

    private readonly SpawnEngineerTurretSkillData data;
    private readonly GameObject source;

    public SpawnEngineerTurretSkill(GameObject source, AbilityController controller, SpawnEngineerTurretSkillData data, Stat cooldownTime, Stat maxSummons) : base(controller, data, cooldownTime, maxSummons)
    {
        this.source = source;
        this.data = data;
    }

    public override bool CanActivate()
    {
        return !DurationActive && !IsBlocked(WorldSpawnPosition, data.SpawnBlockRadius, data.SpawnBlockLayers);
    }

    public override bool CanDeactivate()
    {
        return DurationActive;
    }

    protected override void OnActivation()
    {
        if (summons.Count >= MaxSummons)
        {
            DestroySummon(summons[0]);
        }

        if (TrySpawnSummon(data.SummonPrefab, out EngineerTurret engineerTurret))
        {
            engineerTurret.SetData(source);
        }
    }

    protected override void OnDeactivation() { }

    public override void OnDestroy()
    {
        DestroyAllSummoned();
    }

    protected override bool CanUpgrade()
    {
        return !DurationActive;
    }

    protected override void UpgradeInternal()
    {
        UpgradeStats();
    }

    private void UpgradeStats()
    {
        UpgradeSummonStats();
    }
}