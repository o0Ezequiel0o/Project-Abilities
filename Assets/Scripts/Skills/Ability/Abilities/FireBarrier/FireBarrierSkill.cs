using UnityEngine;

public class FireBarrierSkill : AbilityBase
{
    public override AbilityData Data => data;

    private readonly FireBarrierSkillData data;
    private readonly GameObject source;

    private readonly Stat duration;

    private readonly GameObjectPool<FireBarrier> fireBarrierPool = new GameObjectPool<FireBarrier>();

    public FireBarrierSkill(GameObject source, AbilityController controller, FireBarrierSkillData data, Stat cooldownTime, Stat duration) : base(controller, cooldownTime)
    {
        this.source = source;
        this.data = data;

        this.duration = duration;
    }

    public override bool CanActivate()
    {
        return !DurationActive;
    }

    public override bool CanDeactivate()
    {
        return DurationActive;
    }

    protected override void OnActivation()
    {
        FireBarrier fireBarrier = fireBarrierPool.Get(data.FireBarrierPrefab);

        Vector3 spawnPosition = controller.CastWorldPosition + data.CastDistanceAway * controller.CastDirection;
        fireBarrier.SetValues(spawnPosition, source, controller.CastDirection, data.Size * data.Scale, duration.Value);

        fireBarrier.gameObject.SetActive(true);
    }

    protected override void OnDeactivation() { }

    protected override bool CanUpgrade()
    {
        return !DurationActive;
    }

    protected override void UpgradeInternal()
    {
        UpgradeStats();
    }

    public override void OnDestroy()
    {
        fireBarrierPool.Clear();
    }

    protected override void UpdateActive()
    {
        TryDeactivate();
    }

    private void UpgradeStats()
    {
        cooldownTime.Upgrade();
    }
}