using System.Collections.Generic;
using UnityEngine;

public abstract class SpinnerBaseSkill<T> : AbilityBase where T : Component
{
    private readonly GameObject prefab;
    private readonly GameObject source;

    protected Spinner<T> spinnerInstance;

    protected readonly Stat distance;
    protected readonly Stat speed;
    protected readonly Stat amount;

    public SpinnerBaseSkill(GameObject source, AbilityController controller, SpinnerBaseSkillData data, Stat cooldownTime, Stat duration, Stat distance, Stat speed, Stat amount) : base(controller, cooldownTime, duration)
    {
        prefab = data.SpinObjectPrefab;
        this.source = source;

        this.distance = distance;
        this.speed = speed;
        this.amount = amount;
    }

    protected virtual void OnSpinnerInitialization(List<T> spawnedObjects) { }

    protected override void Awake()
    {
        spinnerInstance = new Spinner<T>();
        spinnerInstance.onInitialization += OnSpinnerInitialization;
    }

    protected void InitializeSpinner(float distance, float speed, int amount)
    {
        if (prefab.TryGetComponent(out T prefabComponent))
        {
            spinnerInstance.InitializeSpinner(null, prefabComponent, distance, speed, amount);
        }
    }

    protected void DestroySpinner()
    {
        spinnerInstance?.Destroy();
        spinnerInstance = null;
    }

    protected override void UpdateActive()
    {
        spinnerInstance?.Update();
    }

    protected override void LateUpdateAll()
    {
        if (spinnerInstance.Pivot != null)
        {
            spinnerInstance.Pivot.transform.position = source.transform.position;
        }
    }

    public override void OnDestroy()
    {
        spinnerInstance?.Destroy();
        spinnerInstance = null;
    }

    protected override void UpgradeInternal()
    {
        distance.Upgrade();
        speed.Upgrade();
        amount.Upgrade();
    }
}