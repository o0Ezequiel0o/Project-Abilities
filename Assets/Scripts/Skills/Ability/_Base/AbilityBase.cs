using UnityEngine;

public abstract class AbilityBase : IAbility
{
    public abstract AbilityData Data { get; }

    public int Level { get; private set; }

    public int MaxCharges { get; private set; }
    public int Charges { get; private set; }

    public virtual float ChargePercentage
    {
        get
        {
            if (CooldownTime <= 0f)
            {
                return 1f;
            }

            return Mathf.Clamp01(CooldownTimer / CooldownTime);
        }
    }

    public virtual float DurationPercentage
    {
        get
        {
            if (DurationTime <= 0f)
            {
                return 1f;
            }

            return Mathf.Clamp01(DurationTimer / DurationTime);
        }
    }

    public float CooldownTime => cooldownTime.Value * CooldownMultiplier;
    public float CooldownTimer { get; protected set; }

    public float DurationTime => durationTime.Value;
    public float DurationTimer { get; protected set; }

    public bool DurationActive { get; protected set; }

    public bool HasCharges => Charges > 0;
    public bool OnCooldown => CooldownTime > CooldownTimer && Charges < 1;

    protected bool UsesDuration => DurationTime > 0f;

    protected readonly AbilityController controller;

    protected readonly Stat cooldownTime;
    protected readonly Stat durationTime;

    private float CooldownMultiplier => controller.abilityCooldownMultiplier[Data.AbilityType].Value;

    private int pendingUpgrades = 0;

    public AbilityBase(AbilityController controller, Stat cooldownTime)
    {
        this.controller = controller;
        this.cooldownTime = cooldownTime;

        durationTime = Stat.Zero;
    }

    public AbilityBase(AbilityController controller, Stat cooldownTime, Stat durationTime)
    {
        this.controller = controller;
        this.cooldownTime = cooldownTime;
        this.durationTime = durationTime;
    }

    public void Initialize()
    {
        CooldownTimer = CooldownTime;
        SetValuesDefault();
        Awake();
    }

    public virtual void SetCharges(int amount)
    {
        if (Charges <= MaxCharges)
        {
            Charges = Mathf.Clamp(amount, 0, MaxCharges);
        }

        if (Charges == MaxCharges)
        {
            CooldownTimer = CooldownTime;
        }
    }

    public virtual void SetCooldownTimer(float amount)
    {
        if (!DurationActive)
        {
            CooldownTimer = Mathf.Max(0f, amount);
        }
    }

    public virtual void SetDuration(int amount)
    {
        DurationTimer = Mathf.Max(0f, amount);
    }

    public void ForceActivate()
    {
        OnActivation();

        if (Charges == MaxCharges || Charges == 1)
        {
            CooldownTimer = 0f;
        }
        
        Charges -= 1;

        if (UsesDuration)
        {
            DurationActive = true;
        }
    }

    public void ForceDeactivate()
    {
        OnDeactivation();

        if (UsesDuration)
        {
            DurationActive = false;
        }
    }

    public void TryActivate()
    {
        if (CanActivate() && CanActivateBase())
        {
            ForceActivate();
        }
    }

    public void TryDeactivate()
    {
        if (CanDeactivate())
        {
            ForceDeactivate();
        }
    }

    public void Upgrade()
    {
        if (CanUpgrade())
        {
            BaseUpgrade();
            UpgradeInternal();
            Level += 1;
        }
        else
        {
            pendingUpgrades += 1;
        }
    }

    public void Update()
    {
        CheckPendingUpgrades();
        UpdateAll();

        if (DurationActive)
        {
            UpdateActiveBase();
            UpdateActive();
        }
        else
        {
            UpdateUnactiveBase();
            UpdateUnactive();
        }
    }

    public void LateUpdate()
    {
        LateUpdateAll();
    }

    public abstract bool CanActivate();

    public abstract bool CanDeactivate();

    public virtual void OnDestroy() { }

    protected abstract bool CanUpgrade();

    protected virtual void Awake() { }

    protected virtual void OnActivation() { }

    protected virtual void OnDeactivation() { }

    protected virtual void OnDurationFinished() { }

    protected virtual void UpdateAll() { }

    protected virtual void LateUpdateAll() { }

    protected virtual void UpdateActive() { }

    protected virtual void UpdateUnactive() { }

    protected virtual void UpgradeInternal() { }

    protected void SetMaxCharges(int amount)
    {
        MaxCharges = Mathf.Max(amount, 0);

        if (MaxCharges == 0)
        {
            CooldownTimer = CooldownTime;
        }
    }

    protected virtual void UpdateActiveBase()
    {
        DurationTimer += Time.deltaTime;

        if (DurationTimer > DurationTime)
        {
            OnDurationFinished();
            ForceDeactivate();

            DurationTimer = 0f;
        }
    }

    protected virtual void UpdateUnactiveBase()
    {
        if (Charges == MaxCharges) return;

        CooldownTimer += Time.deltaTime;

        if (CooldownTimer > CooldownTime && Charges < MaxCharges)
        {
            Charges += 1;

            if (Charges < MaxCharges)
            {
                CooldownTimer = 0f;
            }
        }
    }

    private void CheckPendingUpgrades()
    {
        if (pendingUpgrades == 0) return;
        if (CanUpgrade() == false) return;

        for (int i = 0; i < pendingUpgrades; i++)
        {
            BaseUpgrade();
            UpgradeInternal();
            Level += 1;

            pendingUpgrades -= 1;
        }
    }

    private void SetValuesDefault()
    {
        Level = 1;
        Charges = 1;
        MaxCharges = 1;

        DurationTimer = 0f;

        DurationActive = false;
    }

    private bool CanActivateBase()
    {
        return HasCharges;
    }

    private void BaseUpgrade()
    {
        cooldownTime.Upgrade();
        durationTime.Upgrade();
    }
}