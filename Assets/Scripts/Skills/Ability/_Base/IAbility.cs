public interface IAbility
{
    public abstract AbilityData Data { get; }

    public int Level { get; }

    public int MaxCharges { get; }
    public int Charges { get; }

    public abstract float ChargePercentage { get; }
    public abstract float DurationPercentage { get; }

    public abstract float CooldownTime { get; }
    public abstract float CooldownTimer { get; }

    public abstract float DurationTime { get; }
    public abstract float DurationTimer { get; }

    public abstract bool DurationActive { get; }

    public bool HasCharges { get; }
    public bool OnCooldown { get; }

    public abstract void SetCooldownTimer(float amount);

    public abstract void SetCharges(int amount);

    public abstract void SetDuration(int amount);

    public abstract void Initialize();

    public abstract void ForceActivate();

    public abstract void ForceDeactivate();

    public abstract bool CanActivate();

    public abstract bool CanDeactivate();

    public abstract void TryActivate();

    public abstract void TryDeactivate();

    public abstract void Update();

    public abstract void LateUpdate();

    public abstract void OnDestroy();

    public abstract void Upgrade();
}