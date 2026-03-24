using System.Collections.Generic;
using UnityEngine;
using System;

public class AbilityController : MonoBehaviour, IUpgradable
{
    [Header("Settings")]
    [SerializeField] private Transform castPosition;
    [SerializeField] private List<AbilityData> setAbilities;

    public readonly Dictionary<AbilityType, Stat> abilityCooldownMultiplier = new Dictionary<AbilityType, Stat>
    {
        {AbilityType.Primary, new Stat(1f, 0f, 0f, float.PositiveInfinity)},
        {AbilityType.Secondary, new Stat(1f, 0f, 0f, float.PositiveInfinity)},
        {AbilityType.Utility, new Stat(1f, 0f, 0f, float.PositiveInfinity)},
        {AbilityType.Ultimate, new Stat(1f, 0f, 0f, float.PositiveInfinity)}
    };

    public List<IAbility> Abilities => abilityList;

    /// <summary> Called before ability is used. Returns AbilityType and if input is being held. </summary>
    public Action<AbilityType, bool> onUseAbility;
    
    public Action<IAbility> onAbilityUsed;
    public Action<IAbility> onAbilityAdded;
    public Action<IAbility> onAbilityRemoved;

    public Vector3 CastWorldPosition => castPosition.position;
    public Quaternion CastWorldRotation => castPosition.rotation;
    public Vector3 CastDirection => castPosition.up;

    private readonly List<IAbility> abilityList = new List<IAbility>();

    private readonly Dictionary<AbilityType, IAbility> abilities = new Dictionary<AbilityType, IAbility>
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

    public bool TryGetAbility(AbilityType abilityType, out IAbility ability)
    {
        if (abilities.TryGetValue(abilityType, out ability))
        {
            return ability != null;
        }

        return false;
    }

    public bool CanUseAbility(AbilityType abilityType)
    {
        if (abilities.TryGetValue(abilityType, out IAbility ability))
        {
            return CanUseAbility(ability, abilityType);
        }

        return false;
    }

    public bool CanUseAbility(IAbility ability, AbilityType abilityType)
    {
        return ability.CanActivate();
    }

    public void AddAbility(AbilityData abilityData)
    {
        AddAbility(abilityData.CreateAbility(gameObject, this));
    }

    public void AddAbility(IAbility ability)
    {
        AddAbility(ability, true);
    }

    public void RemoveAbility(AbilityType abilityType)
    {
        RemoveAbility(abilityType, true);
    }

    /// <summary> Returns the replaced ability by the new ability. </summary>
    public IAbility SwitchAbility(AbilityData abilityData)
    {
        return SwitchAbility(abilityData.CreateAbility(gameObject, this), true);
    }

    /// <summary> Returns the replaced ability by the new ability. </summary>
    public IAbility SwitchAbility(IAbility newAbility, bool initialize)
    {
        AbilityType abilityType = newAbility.Data.AbilityType;
        IAbility oldAbility = abilities[abilityType];

        if (initialize) newAbility.Initialize();

        RemoveAbility(abilityType, false);
        AddAbility(newAbility, initialize);

        return oldAbility;
    }

    public void ReplaceAbility(IAbility newAbility)
    {
        AbilityType abilityType = newAbility.Data.AbilityType;
        IAbility oldAbility = abilities[abilityType];

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

        IAbility ability = abilities[abilityType];

        if (ability == null || IsLocked(abilityType)) return;
        if (holding && !ability.Data.CanHold) return;

        UseAbility(ability, abilityType);
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
        castPosition = GetComponentInChildren<Transform>();
    }

    private void Awake()
    {
        if (castPosition == null)
        {
            castPosition = transform;
        }

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
        if (abilities.TryGetValue(abilityType, out IAbility ability))
        {
            RemoveAbility(ability, destroy);
        }
    }

    private void RemoveAbility(IAbility ability, bool destroy)
    {
        if (ability == null) return;

        abilityList.Remove(ability);
        abilities[ability.Data.AbilityType] = null;

        ability.ForceDeactivate();
        if (destroy) ability.OnDestroy();

        onAbilityRemoved?.Invoke(ability);
    }

    private void AddAbility(IAbility ability, bool initialize)
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

    private void UseAbility(IAbility ability, AbilityType abilityType)
    {
        if (CanUseAbility(ability, abilityType))
        {
            ability.TryActivate();
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