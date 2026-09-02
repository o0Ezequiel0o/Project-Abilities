using System;
using System.Collections.Generic;
using UnityEngine;
using Zeke.Abilities;

public class Test : MonoBehaviour
{
    [Header("Remove AbilityType from AbilityData")]
    [SerializeField] private Dictionary<AbilityType, AbilitySlot> abilities;

    [Serializable]
    private class AbilitySlot
    {
        [field: SerializeField] public AbilityData AbilityData { get; private set; }

        public Stat CooldownMultiplier { get; private set; } = new Stat(1, 0, 0, float.PositiveInfinity);
        public Stat RechargeSpeed { get; private set; } = new Stat(1, 0, 0, float.PositiveInfinity);

        public IAbility Ability { get; private set; } = null;
        public List<AbilityLock> Locks { get; private set; } = new List<AbilityLock>();
    }
}