using System.Collections.Generic;
using UnityEngine;
using Zeke.Abilities.Modules;

public class ModularAbility : IModularAbility
{
    public ModularAbilityData Data => data;
    private readonly ModularAbilityData data;

    public int Level { get; private set; }

    public int MaxCharges { get; private set; }
    public int Charges { get; private set; }

    private readonly Transform spawn;
    private readonly GameObject source;

    private readonly List<AbilityModule> modules;

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

    protected readonly ModularAbilityController controller;

    protected readonly Stat cooldownTime;
    protected readonly Stat durationTime;
    protected readonly Stat maxCharges;

    private float CooldownMultiplier => controller.abilityCooldownMultiplier[Data.AbilityType].Value;

    private int pendingUpgrades = 0;

    public ModularAbility(GameObject source, ModularAbilityData data, ModularAbilityController controller, Transform spawn, Stat cooldownTime, Stat durationTime, Stat maxCharges)
    {
        this.data = data;
        this.spawn = spawn;
        this.source = source;
        this.controller = controller;
        modules = new List<AbilityModule>();

        this.cooldownTime = cooldownTime.DeepCopy();
        this.durationTime = durationTime.DeepCopy();
        this.maxCharges = maxCharges.DeepCopy();
    }

    public void Initialize()
    {
        CooldownTimer = CooldownTime;
        SetValuesDefault();
    }

    public void AddModule(AbilityModule module)
    {
        AbilityModule newModule = module.CreateDeepCopy();
        newModule.OnInitialization(controller, spawn, source, this);

        modules.Add(newModule);
    }

    public void AddModules(List<AbilityModule> modules)
    {
        for (int i = 0; i < modules.Count; i++)
        {
            AddModule(modules[i]);
        }
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
        if (DurationActive) return;
        if (Charges == MaxCharges) return;

        CooldownTimer = Mathf.Max(0f, amount);

        if (CooldownTimer > CooldownTime && Charges < MaxCharges)
        {
            Charges += 1;

            if (Charges < MaxCharges)
            {
                CooldownTimer = 0f;
            }
        }
    }

    public virtual void SetDuration(int amount)
    {
        DurationTimer = Mathf.Max(0f, amount);
    }

    public void ForceActivate(bool holding)
    {
        OnActivation(holding);

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

    public void TryActivate(bool holding)
    {
        if (CanActivate() && CanActivateBase())
        {
            ForceActivate(holding);

            if (!DurationActive)
            {
                TryDeactivate();
            }
        }
        else if (DurationActive && data.ManualDeactivation)
        {
            TryDeactivate();
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

    public bool CanActivate()
    {
        for (int i = 0; i < modules.Count; i++)
        {
            if (!modules[i].CanActivate())
            {
                return false;
            }
        }

        return true;
    }

    public bool CanDeactivate()
    {
        for (int i = 0; i < modules.Count; i++)
        {
            if (!modules[i].CanDeactivate())
            {
                return false;
            }
        }

        return true;
    }

    public void OnDestroy()
    {
        for (int i = 0; i < modules.Count; i++)
        {
            modules[i].Destroy();
        }
    }

    protected bool CanUpgrade()
    {
        for (int i = 0; i < modules.Count; i++)
        {
            if (!modules[i].CanUpgrade())
            {
                return false;
            }
        }

        return true;
    }

    protected void OnActivation(bool holding)
    {
        for (int i = 0; i < modules.Count; i++)
        {
            modules[i].Activate(holding);
        }
    }

    protected void OnDeactivation()
    {
        for (int i = 0; i < modules.Count; i++)
        {
            modules[i].Deactivate();
        }
    }

    protected virtual void OnDurationFinished()
    {
        for (int i = 0; i < modules.Count; i++)
        {
            modules[i].OnDurationFinished();
        }
    }

    protected virtual void UpdateAll()
    {
        for (int i = 0; i < modules.Count; i++)
        {
            modules[i].Update();
        }
    }

    protected virtual void LateUpdateAll()
    {
        for (int i = 0; i < modules.Count; i++)
        {
            modules[i].LateUpdate();
        }
    }

    protected void UpdateActive()
    {
        for (int i = 0; i < modules.Count; i++)
        {
            modules[i].UpdateActive();
        }
    }

    protected virtual void UpdateUnactive()
    {
        for (int i = 0; i < modules.Count; i++)
        {
            modules[i].UpdateUnactive();
        }
    }

    protected virtual void UpgradeInternal() { }

    protected void SetMaxCharges(int amount)
    {
        MaxCharges = Mathf.Max(amount, 1);
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

    protected virtual void UpdateUnactiveBase() { }

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

        DurationTimer = 0f;
        DurationActive = false;

        SetMaxCharges(Mathf.FloorToInt(maxCharges.Value));
    }

    private bool CanActivateBase()
    {
        return HasCharges && !DurationActive;
    }

    private void BaseUpgrade()
    {
        cooldownTime.Upgrade();
        durationTime.Upgrade();
        maxCharges.Upgrade();

        SetMaxCharges(Mathf.FloorToInt(maxCharges.Value));
    }
}