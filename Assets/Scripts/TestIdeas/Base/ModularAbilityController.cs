using System;
using System.Collections.Generic;
using UnityEngine;

public class ModularAbilityController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject source;
    [SerializeField] private Transform spawn;
    [SerializeField] private List<ModularAbilityData> setAbilities;

    public readonly Dictionary<AbilityType, Stat> abilityCooldownMultiplier = new Dictionary<AbilityType, Stat>
    {
        {AbilityType.Primary, new Stat(1f, 0f, 0f, float.PositiveInfinity)},
        {AbilityType.Secondary, new Stat(1f, 0f, 0f, float.PositiveInfinity)},
        {AbilityType.Utility, new Stat(1f, 0f, 0f, float.PositiveInfinity)},
        {AbilityType.Ultimate, new Stat(1f, 0f, 0f, float.PositiveInfinity)}
    };

    public List<IModularAbility> Abilities => abilityList;

    /// <summary> Called before ability is used. Returns AbilityType and if input is being held. </summary>
    public Action<AbilityType, bool> onUseAbility;

    public Action<IModularAbility> onAbilityUsed;
    public Action<IModularAbility> onAbilityAdded;
    public Action<IModularAbility> onAbilityRemoved;

    private readonly List<IModularAbility> abilityList = new List<IModularAbility>();

    private readonly Dictionary<AbilityType, IModularAbility> abilities = new Dictionary<AbilityType, IModularAbility>
    {
        {AbilityType.Primary, null},
        {AbilityType.Secondary, null},
        {AbilityType.Utility, null},
        {AbilityType.Ultimate, null}
    };

    private readonly Dictionary<AbilityType, HashSet<AbilityLock>> abilityLocks = new Dictionary<AbilityType, HashSet<AbilityLock>>
    {
        {AbilityType.Primary, new HashSet<AbilityLock>()},
        {AbilityType.Secondary, new HashSet<AbilityLock>()},
        {AbilityType.Utility, new HashSet<AbilityLock>()},
        {AbilityType.Ultimate, new HashSet<AbilityLock>()}
    };

    public bool IsLocked(AbilityType abilityType)
    {
        return abilityLocks[abilityType].Count > 0;
    }

    public bool TryGetAbility(AbilityType abilityType, out IModularAbility ability)
    {
        if (abilities.TryGetValue(abilityType, out ability))
        {
            return ability != null;
        }

        return false;
    }

    public bool CanUseAbility(AbilityType abilityType)
    {
        if (abilities.TryGetValue(abilityType, out IModularAbility ability))
        {
            return CanUseAbility(ability);
        }

        return false;
    }

    public bool CanUseAbility(IModularAbility ability)
    {
        return ability.CanActivate();
    }

    public bool CanDeactivateAbility(AbilityType abilityType)
    {
        if (abilities.TryGetValue(abilityType, out IModularAbility ability))
        {
            return CanDeactivateAbility(ability);
        }

        return false;
    }

    public bool CanDeactivateAbility(IModularAbility ability)
    {
        return ability.CanDeactivate();
    }

    public void AddAbility(ModularAbilityData abilityData)
    {
        AddAbility(abilityData.CreateModularAbility(this, spawn, source));
    }

    public void AddAbility(IModularAbility ability)
    {
        AddAbility(ability, true);
    }

    public void RemoveAbility(AbilityType abilityType)
    {
        RemoveAbility(abilityType, true);
    }

    /// <summary> Returns the replaced ability by the new ability. </summary>
    public IModularAbility SwitchAbility(ModularAbilityData abilityData)
    {
        return SwitchAbility(abilityData.CreateModularAbility(this, spawn, source), true);
    }

    /// <summary> Returns the replaced ability by the new ability. </summary>
    public IModularAbility SwitchAbility(IModularAbility newAbility, bool initialize)
    {
        AbilityType abilityType = newAbility.Data.AbilityType;
        IModularAbility oldAbility = abilities[abilityType];

        if (initialize) newAbility.Initialize();

        RemoveAbility(abilityType, false);
        AddAbility(newAbility, initialize);

        return oldAbility;
    }

    public void ReplaceAbility(IModularAbility newAbility)
    {
        AbilityType abilityType = newAbility.Data.AbilityType;
        IModularAbility oldAbility = abilities[abilityType];

        if (oldAbility != null)
        {
            RemoveAbility(abilityType, true);
            AddAbility(newAbility, true);
        }
    }

    public void TryUseAbility(AbilityType abilityType)
    {
        TryUseAbility(abilityType, false);
    }

    public void TryUseAbility(AbilityType abilityType, bool holding)
    {
        onUseAbility?.Invoke(abilityType, holding);

        IModularAbility ability = abilities[abilityType];

        if (ability == null || IsLocked(abilityType)) return;
        if (holding && !ability.Data.CanHold) return;

        UseAbility(ability, abilityType, holding);
    }

    public void UpgradeAbility(AbilityType abilityType)
    {
        abilities[abilityType]?.Upgrade();
    }

    public void AddAbilityLock(AbilityLock abilityLock)
    {
        if (abilityLocks.TryGetValue(abilityLock.abilityType, out HashSet<AbilityLock> locks))
        {
            locks.Add(abilityLock);
        }
    }

    public void RemoveAbilityLock(AbilityLock abilityLock)
    {
        if (abilityLocks.TryGetValue(abilityLock.abilityType, out HashSet<AbilityLock> locks))
        {
            locks.Remove(abilityLock);
        }
    }

    public void Upgrade()
    {
        for (int i = 0; i < abilityList.Count; i++)
        {
            abilityList[i].Upgrade();
        }
    }

    private void Reset()
    {
        spawn = GetComponentInChildren<Transform>();
    }

    private void Awake()
    {
        for (int i = 0; i < setAbilities.Count; i++)
        {
            AddAbility(setAbilities[i]);
        }
    }

    private void Update()
    {
        UpdateAbilities();
    }

    private void LateUpdate()
    {
        LateUpdateAbilities();
    }

    private void OnDestroy()
    {
        RemoveAbilities();
    }

    private void RemoveAbility(AbilityType abilityType, bool destroy)
    {
        if (abilities.TryGetValue(abilityType, out IModularAbility ability))
        {
            RemoveAbility(ability, destroy);
        }
    }

    private void RemoveAbility(IModularAbility ability, bool destroy)
    {
        if (ability == null) return;

        abilityList.Remove(ability);
        abilities[ability.Data.AbilityType] = null;

        ability.ForceDeactivate();
        if (destroy) ability.OnDestroy();

        onAbilityRemoved?.Invoke(ability);
    }

    private void AddAbility(IModularAbility ability, bool initialize)
    {
        AbilityType abilityType = ability.Data.AbilityType;

        if (abilities[abilityType] == null)
        {
            if (initialize) ability.Initialize();

            abilityList.Add(ability);
            abilities[abilityType] = ability;

            onAbilityAdded?.Invoke(ability);
        }
    }

    private void UseAbility(IModularAbility ability, AbilityType abilityType, bool holding)
    {
        if (CanUseAbility(ability))
        {
            ability.TryActivate(holding);
            onAbilityUsed?.Invoke(ability);
        }
    }

    private AbilityType IntToAbilityType(int value)
    {
        return value switch
        {
            0 => AbilityType.Primary,
            1 => AbilityType.Secondary,
            2 => AbilityType.Utility,
            3 => AbilityType.Ultimate,
            _ => AbilityType.Primary,
        };
    }

    private void UpdateAbilities()
    {
        for (int i = 0; i < Abilities.Count; i++)
        {
            Abilities[i].Update();
        }
    }

    private void LateUpdateAbilities()
    {
        for (int i = 0; i < Abilities.Count; i++)
        {
            Abilities[i].LateUpdate();
        }
    }

    private void RemoveAbilities()
    {
        for (int i = Abilities.Count - 1; i >= 0; i--)
        {
            RemoveAbility(Abilities[i], true);
        }
    }
}

public interface IModularAbility
{
    public abstract ModularAbilityData Data { get; }

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

    public abstract void ForceActivate(bool holding);

    public abstract void ForceDeactivate();

    public abstract bool CanActivate();

    public abstract bool CanDeactivate();

    public abstract void TryActivate(bool holding);

    public abstract void TryDeactivate();

    public abstract void Update();

    public abstract void LateUpdate();

    public abstract void OnDestroy();

    public abstract void Upgrade();
}