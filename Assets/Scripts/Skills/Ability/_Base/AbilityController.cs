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

        public OrderedAction<IAbility, AbilityType> onAbilityUsed = new OrderedAction<IAbility, AbilityType>();
        public OrderedAction<IAbility, AbilityType> onAbilityCharged = new OrderedAction<IAbility, AbilityType>();

        public Action<IAbility, AbilityType> onAbilityAdded;
        public Action<IAbility, AbilityType> onAbilityRemoved;

        /// <summary> Called before ability is used. Returns AbilityType and if input is being held. </summary>
        public Action<AbilityType, bool> onUseAbility;

        public Dictionary<AbilityType, AbilitySlot> Abilities { get; private set; }  = new Dictionary<AbilityType, AbilitySlot>
        {
            {AbilityType.Primary, new AbilitySlot()},
            {AbilityType.Secondary, new AbilitySlot()},
            {AbilityType.Utility, new AbilitySlot()},
            {AbilityType.Ultimate, new AbilitySlot()}
        };

        private readonly Dictionary<AbilityData, AbilityType> linkedAbilityData = new Dictionary<AbilityData, AbilityType>();

        private int level = 1;

        public class AbilitySlot
        {
            public AbilityData AbilityData { get; internal set; }
            public IAbility Ability { get; internal set; }

            public Stat CooldownMultiplier { get; private set; } = new Stat(1f, 0f, 0f, float.PositiveInfinity);
            public Stat RechargeSpeed { get; private set; } = new Stat(1f, 0f, 0f, float.PositiveInfinity);

            public HashSet<AbilityLock> AbilityLocks { get; internal set; } = new HashSet<AbilityLock>();
        }

        public AbilityType GetAbilityType(AbilityData abilityData)
        {
            if (linkedAbilityData.TryGetValue(abilityData, out AbilityType abilityType))
            {
                return abilityType;
            }
            else
            {
                throw new Exception($"{abilityData} is not linked to any ability slot inside");
            }
        }

        public bool IsLocked(AbilityType abilityType)
        {
            return Abilities[abilityType].AbilityLocks.Count > 0;
        }

        public bool TryGetAbility(AbilityData abilityData, out IAbility ability)
        {
            ability = null;

            foreach(AbilitySlot abilitySlot in Abilities.Values)
            {
                if (abilitySlot.AbilityData == abilityData)
                {
                    ability = abilitySlot.Ability;
                    return true;
                }
            }

            return false;
        }

        public bool TryGetAbility(AbilityType abilityType, out IAbility ability)
        {
            ability = Abilities[abilityType].Ability;
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

        public void AddAbility(AbilityData abilityData, AbilityType abilityType)
        {
            AddAbility(abilityData.CreateModularAbility(this, spawn, gameObject), abilityType);
        }

        public void AddAbility(IAbility ability, AbilityType abilityType)
        {
            AddAbility(ability, abilityType, true);
        }

        public void RemoveAbility(AbilityType abilityType)
        {
            RemoveAbility(abilityType, true);
        }

        /// <summary> Returns the replaced ability by the new ability. </summary>
        public IAbility SwitchAbility(AbilityData abilityData, AbilityType abilityType)
        {
            return SwitchAbility(abilityData.CreateModularAbility(this, spawn, gameObject), abilityType, true);
        }

        /// <summary> Returns the replaced ability by the new ability. </summary>
        public IAbility SwitchAbility(IAbility newAbility, AbilityType abilityType, bool initialize)
        {
            IAbility oldAbility = Abilities[abilityType].Ability;

            RemoveAbility(abilityType, false);
            AddAbility(newAbility, abilityType, initialize);

            return oldAbility;
        }

        public void ReplaceAbility(IAbility newAbility, AbilityType abilityType)
        {
            IAbility oldAbility = Abilities[abilityType].Ability;

            if (oldAbility != null)
            {
                RemoveAbility(abilityType, true);
                AddAbility(newAbility, abilityType, true);
            }
        }

        public void TryUseAbility(AbilityType abilityType)
        {
            TryUseAbility(abilityType, false);
        }

        public void TryUseAbility(AbilityType abilityType, bool holding)
        {
            onUseAbility?.Invoke(abilityType, holding);

            IAbility ability = Abilities[abilityType].Ability;

            if (ability == null || IsLocked(abilityType)) return;
            if (holding && !ability.Data.CanHold) return;

            UseAbility(ability, abilityType, holding);
        }

        public void UpgradeAbility(AbilityType abilityType)
        {
            Abilities[abilityType].Ability?.QueueUpgrade();
        }

        public void AddAbilityLock(AbilityLock abilityLock)
        {
            Abilities[abilityLock.abilityType].AbilityLocks.Add(abilityLock);
        }

        public void RemoveAbilityLock(AbilityLock abilityLock)
        {
            Abilities[abilityLock.abilityType].AbilityLocks.Remove(abilityLock);
        }

        public void Upgrade()
        {
            foreach (AbilitySlot abilitySlot in Abilities.Values)
            {
                abilitySlot.Ability?.QueueUpgrade();
            }

            level += 1;
        }

        private void Reset()
        {
            spawn = GetComponentInChildren<Transform>();
        }

        private void Awake()
        {
            foreach (AbilityType abilityType in StartAbilities.Keys)
            {
                AddAbility(StartAbilities[abilityType], abilityType);
            }
        }

        private void Update()
        {
            foreach (AbilitySlot abilitySlot in Abilities.Values)
            {
                abilitySlot.Ability?.Update();
            }
        }

        private void LateUpdate()
        {
            foreach (AbilitySlot abilitySlot in Abilities.Values)
            {
                abilitySlot.Ability?.LateUpdate();
            }
        }

        private void OnDestroy()
        {
            RemoveAbilities();
        }

        private void RemoveAbility(AbilityType abilityType, bool destroy)
        {
            RemoveAbility(Abilities[abilityType].Ability, destroy);
        }

        private void RemoveAbility(IAbility ability, bool destroy)
        {
            if (ability == null) return;

            foreach (AbilityType abilityType in Abilities.Keys)
            {
                if (Abilities[abilityType].Ability != ability) continue;

                linkedAbilityData.Remove(ability.Data);

                Abilities[abilityType].AbilityData = null;
                Abilities[abilityType].Ability = null;

                ability.TryDeactivate();
                if (destroy) ability.Destroy();

                onAbilityRemoved?.Invoke(ability, abilityType);

                break;
            }
        }

        private void AddAbility(IAbility ability, AbilityType abilityType, bool initialize)
        {
            if (Abilities[abilityType].Ability == null)
            {
                linkedAbilityData.Add(ability.Data, abilityType);

                if (initialize) ability.Initialize();

                Abilities[abilityType].AbilityData = ability.Data;
                Abilities[abilityType].Ability = ability;

                onAbilityAdded?.Invoke(ability, abilityType);

                while (ability.Level < level)
                {
                    ability.QueueUpgrade();
                }
            }
        }

        private void UseAbility(IAbility ability, AbilityType abilityType, bool holding)
        {
            if (ability.TryActivate(holding))
            {
                onAbilityUsed?.Invoke(ability, abilityType);
            }   
        }

        private void RemoveAbilities()
        {
            foreach (AbilitySlot abilitySlot in Abilities.Values)
            {
                RemoveAbility(abilitySlot.Ability, true);
            }
        }
    }
}