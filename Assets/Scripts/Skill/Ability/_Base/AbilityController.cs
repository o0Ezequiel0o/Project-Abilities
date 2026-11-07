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

        public readonly OrderedAction<IAbility> onAbilityUsed = new OrderedAction<IAbility>();
        public readonly OrderedAction<Ability.ChargeInfo> onAbilityChargesUpdated = new OrderedAction<Ability.ChargeInfo>();

        public Action<IAbility> onAbilityAdded;
        public Action<IAbility> onAbilityRemoved;

        public Action<AbilityOverrideData> onAbilityOverriden;

        public Action<AbilityType, bool> onUseAbility;

        public IReadOnlyDictionary<AbilityType, AbilityTypeSlot> AbilityTypeSlots => abilityTypeSlots;

        private readonly Dictionary<AbilityType, AbilityTypeSlot> abilityTypeSlots = new Dictionary<AbilityType, AbilityTypeSlot>();

        private static readonly AbilityType[] abilityTypes = (AbilityType[])Enum.GetValues(typeof(AbilityType));

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

            public readonly bool overridenRemoved;

            public AbilityOverrideData(IAbility overrideAbility, IAbility overridenAbility, bool overridenRemoved)
            {
                this.overrideAbility = overrideAbility;
                this.overridenAbility = overridenAbility;

                this.overridenRemoved = overridenRemoved;
            }
        }

        public List<IAbility> GetAllAbilities()
        {
            List<IAbility> allAbilities = new List<IAbility>();

            foreach (AbilityTypeSlot abilityTypeSlot in abilityTypeSlots.Values)
            {
                for (int i = 0; i < abilityTypeSlot.Abilities.Count; i++)
                {
                    allAbilities.Add(abilityTypeSlot.Abilities[i]);
                }
            }

            return allAbilities;
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
            abilityTypeSlots[abilityType].Abilities.Add(ability);

            if (ability.Level < level)
            {
                ability.QueueUpgrade(level - ability.Level);
            }

            if (abilityTypeSlots[abilityType].Abilities.Count > 1)
            {
                IAbility overridenAbility = abilityTypeSlots[abilityType].Abilities[^2];
                onAbilityOverriden?.Invoke(new AbilityOverrideData(ability, overridenAbility, false));

                overridenAbility.Disable();
            }

            onAbilityAdded?.Invoke(ability);

            return ability;
        }

        public IAbility RemoveAbility(IAbility ability)
        {
            if (TryGetAbilitySlot(ability, out AbilityTypeSlot abilityTypeSlot, out int index))
            {
                bool wasCurrentAbility = abilityTypeSlot.CurrentAbilitySlot == ability;
                bool hadMultipleAbilitiesInSlot = abilityTypeSlot.Abilities.Count > 1;

                ability.Disable();

                onAbilityRemoved?.Invoke(ability);
                abilityTypeSlot.Abilities.RemoveAt(index);

                if (hadMultipleAbilitiesInSlot && wasCurrentAbility)
                {
                    onAbilityOverriden?.Invoke(new AbilityOverrideData(abilityTypeSlot.CurrentAbilitySlot, ability, true));
                }

                return ability;
            }

            return null;
        }

        public IAbility ReplaceAbility(AbilityData replaceAbilityData, AbilityData newAbilityData)
        {
            if (TryGetAbilitySlot(replaceAbilityData, out AbilityTypeSlot abilityTypeSlot, out int index))
            {
                IAbility replaceAbility = abilityTypeSlot.Abilities[index];

                replaceAbility.Disable();
                onAbilityRemoved?.Invoke(replaceAbility);

                IAbility newAbility = newAbilityData.CreateModularAbility(this, spawn, gameObject, replaceAbility.Type);

                newAbility.Initialize();

                abilityTypeSlot.Abilities[index] = newAbility;

                if (newAbility.Level < level)
                {
                    newAbility.QueueUpgrade(level - newAbility.Level);
                }

                if (abilityTypeSlot == abilityTypeSlot.CurrentAbilitySlot)
                {
                    onAbilityOverriden?.Invoke(new AbilityOverrideData(newAbility, replaceAbility, true));
                }

                onAbilityAdded?.Invoke(newAbility);

                return replaceAbility;
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
                UseAbility(ability, holding);
            }
        }

        public void UpgradeAbility(AbilityData abilityData, int levels)
        {
            if (TryGetAbility(abilityData, out IAbility ability))
            {
                ability.QueueUpgrade(levels);
            }
        }

        public void UpgradeAbility(AbilityType abilityType, int levels)
        {
            if (abilityTypeSlots[abilityType].Abilities.Count == 0) return;

            abilityTypeSlots[abilityType].CurrentAbilitySlot.QueueUpgrade(levels);
        }

        public void AddAbilityLock(AbilityLock abilityLock)
        {
            abilityTypeSlots[abilityLock.abilityType].AbilityLocks.Add(abilityLock);
        }

        public void RemoveAbilityLock(AbilityLock abilityLock)
        {
            abilityTypeSlots[abilityLock.abilityType].AbilityLocks.Remove(abilityLock);
        }

        public void Upgrade(int levels)
        {
            foreach (AbilityTypeSlot abilityTypeSlot in abilityTypeSlots.Values)
            {
                for (int i = 0; i < abilityTypeSlot.Abilities.Count; i++)
                {
                    abilityTypeSlot.Abilities[i]?.QueueUpgrade(levels);
                }
            }

            level += levels;
        }

        private void Reset()
        {
            spawn = GetComponentInChildren<Transform>();
        }

        private void Awake()
        {
            foreach (AbilityType abilityType in abilityTypes)
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
                abilityTypeSlot.CurrentAbilitySlot?.Update();
            }
        }

        private void LateUpdate()
        {
            foreach (AbilityTypeSlot abilityTypeSlot in abilityTypeSlots.Values)
            {
                abilityTypeSlot.CurrentAbilitySlot?.LateUpdate();
            }
        }

        private void OnDestroy()
        {
            DestroyAbilities();
        }

        private bool TryGetAbilitySlot(IAbility ability, out AbilityTypeSlot abilityTypeSlot, out int index)
        {
            abilityTypeSlot = null;
            index = -1;

            foreach (AbilityType abilityType in abilityTypeSlots.Keys)
            {
                List<IAbility> abilitySlots = abilityTypeSlots[abilityType].Abilities;

                for (int i = 0; i < abilitySlots.Count; i++)
                {
                    if (abilitySlots[i] != ability) continue;

                    abilityTypeSlot = abilityTypeSlots[abilityType];
                    index = i;

                    return true;
                }
            }

            return false;
        }

        private bool TryGetAbilitySlot(AbilityData abilityData, out AbilityTypeSlot abilityTypeSlot, out int index)
        {
            abilityTypeSlot = null;
            index = -1;

            foreach (AbilityType abilityType in abilityTypeSlots.Keys)
            {
                List<IAbility> abilitySlots = abilityTypeSlots[abilityType].Abilities;

                for (int i = 0; i < abilitySlots.Count; i++)
                {
                    if (abilitySlots[i].Data != abilityData) continue;

                    abilityTypeSlot = abilityTypeSlots[abilityType];
                    index = i;

                    return true;
                }
            }

            return false;
        }

        private void UseAbility(IAbility ability, bool holding)
        {
            if (ability.TryActivate(holding))
            {
                onAbilityUsed?.Invoke(ability);
            }   
        }

        private void DestroyAbility(IAbility ability)
        {
            RemoveAbility(ability)?.Destroy();
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