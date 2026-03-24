using System.Collections.Generic;
using UnityEngine;

public class HomingOrbsSkill : AbilityBase
{
    public override AbilityData Data => data;

    private readonly HomingOrbsSkillData data;

    private readonly HomingOrb prefab;
    private readonly GameObject source;

    private Spinner<HomingOrb> spinnerInstance;

    protected readonly Stat amount;
    protected readonly Stat damage;
    protected readonly Stat fireCooldown;

    private readonly List<Transform> targetsInRange = new List<Transform>();

    private bool spinnerCreatedThisFrame = false;
    private bool warmUpFinished = false;

    private float fireCooldownTimer = 0f;
    private float warmUpTimer = 0f;

    private List<HomingOrb> homingOrbs;

    private readonly List<RaycastHit2D> targetsInLaunchPath = new List<RaycastHit2D>();
    private readonly List<Collider2D> unfilteredTargetsInRange = new List<Collider2D>();

    public HomingOrbsSkill(GameObject source, AbilityController controller, HomingOrbsSkillData data, Stat cooldownTime, Stat duration, Stat amount, Stat fireCooldown, Stat damage) : base(controller, cooldownTime, duration)
    {
        prefab = data.Projectile;
        this.source = source;
        this.data = data;

        this.amount = amount;
        this.damage = damage;
        this.fireCooldown = fireCooldown;
    }

    protected override void Awake()
    {
        spinnerInstance = new Spinner<HomingOrb>();
        spinnerInstance.onInitialization += OnSpinnerInitialization;
    }

    protected virtual void OnSpinnerInitialization(List<HomingOrb> spawnedObjects)
    {
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            spawnedObjects[i].Launch(spawnedObjects[i].transform.position, 0f, Vector2.zero, Mathf.Infinity, damage.Value, source);
        }

        spinnerCreatedThisFrame = true;
        homingOrbs = spawnedObjects;
        ResetWarmUp();
    }

    protected void DestroySpinner()
    {
        spinnerInstance?.Destroy();
        spinnerInstance = null;
    }

    protected override void UpdateActive()
    {
        if (spinnerInstance == null) return;

        spinnerInstance.Update();

        if (spinnerCreatedThisFrame)
        {
            spinnerCreatedThisFrame = false;
            return;
        }

        UpdateWarmUp();
        UpdateTimers();
        UpdateFiringState();
    }

    protected override void LateUpdateAll()
    {
        if (spinnerInstance.Pivot != null)
        {
            spinnerInstance.Pivot.transform.position = source.transform.position;
        }
    }

    public override bool CanActivate()
    {
        return !DurationActive;
    }

    public override bool CanDeactivate()
    {
        return DurationActive;
    }

    public override void OnDestroy()
    {
        spinnerInstance?.Destroy();
        spinnerInstance = null;
    }

    protected override void OnActivation()
    {
        spinnerInstance.InitializeSpinner(null, prefab, data.Distance, data.SpinSpeed, Mathf.FloorToInt(amount.Value));
    }

    protected override void OnDeactivation()
    {
        spinnerInstance.DisablePivotChildren();
    }

    protected override void OnDurationFinished()
    {
        spinnerInstance.DisablePivotChildren();
    }

    protected override bool CanUpgrade()
    {
        return !DurationActive;
    }

    protected override void UpgradeInternal()
    {
        amount.Upgrade();
        damage.Upgrade();
        fireCooldown.Upgrade();
    }

    private void UpdateWarmUp()
    {
        if (warmUpFinished) return;

        if (warmUpTimer >= data.WarmUp)
        {
            warmUpFinished = true;
        }
    }

    private void UpdateTimers()
    {
        fireCooldownTimer += Time.deltaTime;
        warmUpTimer += Time.deltaTime;
    }

    private void UpdateFiringState()
    {
        if (fireCooldownTimer <= fireCooldown.Value) return;
        if (spinnerInstance.Pivot.childCount <= 0) return;
        if (!warmUpFinished) return;

        fireCooldownTimer = 0f;

        UpdateTargetsInRange();
        TryFireClosestOrbToTargets(targetsInRange, source, data.HitLayers, data.BlockLayers);
    }

    private void UpdateTargetsInRange()
    {
        targetsInRange.Clear();

        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = data.HitLayers };
        Physics2D.OverlapCircle(source.transform.position, data.DetectRadius, contactFilter, unfilteredTargetsInRange);

        for (int i = 0; i < unfilteredTargetsInRange.Count; i++)
        {
            if (unfilteredTargetsInRange[i].gameObject == source) continue;
            if (TeamManager.IsAlly(unfilteredTargetsInRange[i].gameObject, source)) continue;

            targetsInRange.Add(unfilteredTargetsInRange[i].transform);
        }
    }

    public bool TryFireClosestOrbToTargets(List<Transform> targets, GameObject source, LayerMask hitLayers, LayerMask blockLayers)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (TryGetClosestValidOrbToTarget(targets[i], source, hitLayers, blockLayers, out HomingOrb homingOrb))
            {
                FireOrb(homingOrb, targets[i], source);
                return true;
            }
        }

        return false;
    }

    private bool TryGetClosestValidOrbToTarget(Transform target, GameObject source, LayerMask hitLayers, LayerMask blockLayers, out HomingOrb closestOrb)
    {
        closestOrb = null;
        float closestDistance = float.PositiveInfinity;

        for (int i = homingOrbs.Count - 1; i >= 0; i--)
        {
            if (homingOrbs[i] == null)
            {
                homingOrbs.RemoveAt(i);
                continue;
            }

            float distance = (target.transform.position - homingOrbs[i].transform.position).sqrMagnitude;

            if (distance < closestDistance && ValidOrbLaunch(homingOrbs[i], target, source, hitLayers, blockLayers))
            {
                closestOrb = homingOrbs[i];
                closestDistance = distance;
            }
        }

        return closestOrb != null;
    }

    private void FireOrb(HomingOrb homingOrb, Transform target, GameObject source)
    {
        Vector3 direction = (target.transform.position - homingOrb.transform.position).normalized;

        homingOrbs.Remove(homingOrb);
        spinnerInstance.RemoveFromPivot(homingOrb.transform);

        homingOrb.Launch(homingOrb.transform.position, 5f, direction, data.MaxRange, damage.Value, source);

        homingOrb.SetTarget(target);
        homingOrb.EnableCollider();
    }

    protected bool InLayerMask(GameObject hit, LayerMask layerMask)
    {
        return (layerMask & 1 << hit.layer) != 0;
    }

    private bool ValidOrbLaunch(HomingOrb homingOrb, Transform target, GameObject source, LayerMask hitLayers, LayerMask blockLayers)
    {
        targetsInLaunchPath.Clear();

        Vector3 direction = (target.transform.position - homingOrb.transform.position).normalized;
        float distance = Vector3.Distance(homingOrb.transform.position, target.transform.position);

        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = hitLayers | blockLayers };
        Physics2D.CircleCast(homingOrb.transform.position, homingOrb.Radius, direction, contactFilter, targetsInLaunchPath, distance);

        for (int i = 0; i < targetsInLaunchPath.Count; i++)
        {
            if (targetsInLaunchPath[i].transform.gameObject == source) return false;
            if (InLayerMask(targetsInLaunchPath[i].transform.gameObject, blockLayers)) return false;
        }

        return true;
    }

    private void ResetWarmUp()
    {
        warmUpFinished = false;
        warmUpTimer = 0f;
    }
}