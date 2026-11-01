using System.Collections.Generic;
using UnityEngine;
using System;
using Zeke.Collections;

namespace Zeke.Abilities
{
    public class AbilityController : MonoBehaviour, IUpgradable
    {
        [Header("Settings")]
        [SerializeField] private Transform spawn;
        [field: SerializeField] public Dictionary<AbilityType, AbilityData> StartAbilities { get; private set; }

        public Transform Spawn => spawn;

        public readonly OrderedAction<IAbility, AbilityType> onAbilityUsed = new OrderedAction<IAbility, AbilityType>();
        public readonly OrderedAction<IAbility, AbilityType> onAbilityCharged = new OrderedAction<IAbility, AbilityType>();

        public Action<IAbility, AbilityType> onAbilityAdded;
        public Action<IAbility, AbilityType> onAbilityRemoved;
        public Action<AbilityOverrideData> onAbilityOverriden;

        public Action<AbilityType, bool> onUseAbility;

        public IReadOnlyDictionary<AbilityType, AbilityTypeSlot> AbilityTypeSlots => abilityTypeSlots;

        private readonly Dictionary<AbilityType, AbilityTypeSlot> abilityTypeSlots = new Dictionary<AbilityType, AbilityTypeSlot>();
        private readonly Dictionary<AbilityData, AbilityType> linkedAbilityData = new Dictionary<AbilityData, AbilityType>();

        private int level = 1;

        public class AbilityTypeSlot
        {
            public IAbility CurrentAbilitySlot => Abilities.Count == 0 ? null : Abilities[^1];
            public List<IAbility> Abilities { get; internal set; } = new List<IAbility>();

            public AbilityType AbilityType { get; private set; }

            public Stat CooldownMultiplier { get; private set; } = new Stat(1f, 0f, 0f, float.PositiveInfinity);
            public Stat RechargeSpeed { get; private set; } = new Stat(1f, 0f, 0f, float.PositiveInfinity);

            public HashSet<AbilityLock> AbilityLocks { get; internal set; } = new HashSet<AbilityLock>();

            public AbilityTypeSlot(AbilityType abilityType)
            {
                AbilityType = abilityType;
            }
        }

        public readonly struct AbilityOverrideData
        {
            public readonly IAbility overrideAbility;
            public readonly IAbility overridenAbility;

            public AbilityOverrideData(IAbility overrideAbility, IAbility overridenAbility)
            {
                this.overrideAbility = overrideAbility;
                this.overridenAbility = overridenAbility;
            }
        }

        public AbilityTypeSlot GetAbilityTypeSlot(AbilityType abilityType)
        {
            return abilityTypeSlots[abilityType];
        }

        public bool IsLocked(AbilityType abilityType)
        {
            return abilityTypeSlots[abilityType].AbilityLocks.Count > 0;
        }

        public bool TryGetAbility(AbilityData abilityData, out IAbility ability)
        {
            ability = null;

            foreach (AbilityTypeSlot abilityTypeSlot in abilityTypeSlots.Values)
            {
                for (int i = 0; i < abilityTypeSlot.Abilities.Count; i++)
                {
                    if (abilityTypeSlot.Abilities[i].Data == abilityData)
                    {
                        ability = abilityTypeSlot.CurrentAbilitySlot;
                        return ability != null;
                    }
                }
            }

            return false;
        }

        public bool TryGetAbility(AbilityType abilityType, out IAbility ability)
        {
            ability = null;

            if (abilityTypeSlots[abilityType].Abilities.Count == 0)
            {
                return false;
            }

            ability = abilityTypeSlots[abilityType].CurrentAbilitySlot;

            return ability != null;
        }

        public bool CanUseAbility(AbilityType abilityType)
        {
            if (TryGetAbility(abilityType, out IAbility ability))
            {
                return CanUseAbility(ability);
            }

            return false;
        }

        public bool CanUseAbility(IAbility ability)
        {
            return ability.CanActivate();
        }

        public IAbility AddAbility(AbilityData abilityData, AbilityType abilityType)
        {
            return AddAbility(abilityData.CreateModularAbility(this, spawn, gameObject, abilityType), abilityType);
        }

        private IAbility AddAbility(IAbility ability, AbilityType abilityType)
        {
            ability.Initialize();
            return AddInitializedAbility(ability, abilityType);
        }

        public IAbility AddInitializedAbility(IAbility ability, AbilityType abilityType)
        {
            linkedAbilityData.Add(ability.Data, abilityType);
            abilityTypeSlots[abilityType].Abilities.Add(ability);

            while (ability.Level < level)
            {
                ability.QueueUpgrade();
            }

            if (abilityTypeSlots[abilityType].Abilities.Count > 1)
            {
                IAbility overridenAbility = abilityTypeSlots[abilityType].Abilities[1];
                onAbilityOverriden?.Invoke(new AbilityOverrideData(ability, overridenAbility));

                overridenAbility.ForceDeactivate();
            }

            onAbilityAdded?.Invoke(ability, abilityType);

            return ability;
        }

        public IAbility RemoveAbility(IAbility ability)
        {
            foreach (AbilityType abilityType in abilityTypeSlots.Keys)
            {
                List<IAbility> abilitySlots = abilityTypeSlots[abilityType].Abilities;

                for (int i = 0; i < abilitySlots.Count; i++)
                {
                    if (abilitySlots[i] != ability) continue;

                    linkedAbilityData.Remove(ability.Data);
                    ability.ForceDeactivate();

                    onAbilityRemoved?.Invoke(ability, abilityType);
                    abilitySlots.RemoveAt(i);

                    return ability;
                }
            }

            return null;
        }

        public void TryUseAbility(AbilityType abilityType)
        {
            TryUseAbility(abilityType, false);
        }

        public void TryUseAbility(AbilityType abilityType, bool holding)
        {
            if (TryGetAbility(abilityType, out IAbility ability))
            {
                if (ability == null || IsLocked(abilityType)) return;
                if (holding && !ability.Data.CanHold) return;

                onUseAbility?.Invoke(abilityType, holding);
                UseAbility(ability, abilityType, holding);
            }
        }

        public void UpgradeAbility(AbilityData abilityData)
        {
            if (TryGetAbility(abilityData, out IAbility ability))
            {
                ability.QueueUpgrade();
            }
        }

        public void UpgradeAbility(AbilityType abilityType)
        {
            if (abilityTypeSlots[abilityType].Abilities.Count == 0) return;

            abilityTypeSlots[abilityType].CurrentAbilitySlot.QueueUpgrade();
        }

        public void AddAbilityLock(AbilityLock abilityLock)
        {
            abilityTypeSlots[abilityLock.abilityType].AbilityLocks.Add(abilityLock);
        }

        public void RemoveAbilityLock(AbilityLock abilityLock)
        {
            abilityTypeSlots[abilityLock.abilityType].AbilityLocks.Remove(abilityLock);
        }

        public void Upgrade()
        {
            foreach (AbilityTypeSlot abilityTypeSlot in abilityTypeSlots.Values)
            {
                for (int i = 0; i < abilityTypeSlot.Abilities.Count; i++)
                {
                    abilityTypeSlot.Abilities[i]?.QueueUpgrade();
                }
            }

            level += 1;
        }

        private void Reset()
        {
            spawn = GetComponentInChildren<Transform>();
        }

        private void Awake()
        {
            foreach (AbilityType abilityType in Enum.GetValues(typeof(AbilityType)))
            {
                abilityTypeSlots.Add(abilityType, new AbilityTypeSlot(abilityType));
            }

            foreach (AbilityType abilityType in StartAbilities.Keys)
            {
                AddAbility(StartAbilities[abilityType], abilityType);
            }
        }

        private void Update()
        {
            foreach (AbilityTypeSlot abilityTypeSlot in abilityTypeSlots.Values)
            {
                for (int i = 0; i < abilityTypeSlot.Abilities.Count; i++)
                {
                    abilityTypeSlot.Abilities[i]?.Update();
                }
            }
        }

        private void LateUpdate()
        {
            foreach (AbilityTypeSlot abilityTypeSlot in abilityTypeSlots.Values)
            {
                for (int i = 0; i < abilityTypeSlot.Abilities.Count; i++)
                {
                    abilityTypeSlot.Abilities[i]?.LateUpdate();
                }
            }
        }

        private void OnDestroy()
        {
            DestroyAbilities();
        }

        private void DestroyAbility(IAbility ability)
        {
            RemoveAbility(ability)?.Destroy();
        }

        private void UseAbility(IAbility ability, AbilityType abilityType, bool holding)
        {
            if (ability.TryActivate(holding))
            {
                onAbilityUsed?.Invoke(ability, abilityType);
            }   
        }

        private void DestroyAbilities()
        {
            foreach (AbilityTypeSlot abilityTypeSlot in abilityTypeSlots.Values)
            {
                for (int i = 0; i < abilityTypeSlot.Abilities.Count; i++)
                {
                    DestroyAbility(abilityTypeSlot.Abilities[i]);
                }
            }
        }
    }
}