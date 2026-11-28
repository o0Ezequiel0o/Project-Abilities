using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Characters
{
    [Serializable]
    public class OverclockData : AbilityModuleData
    {
        [field: SerializeField] public List<AbilityType> AffectedAbilities { get; private set; }

        [field: Header("Settings")]
        [field: SerializeField] public float TickDuration { get; private set; } = 1f;

        [field: Header("Recharge Boost")]
        [Tooltip("The base ratio bonus for recharge speed")]
        [SerializeField] private Stat baseRechargeRatioBoost = new Stat(0.09f, 0.01f, 0f, float.PositiveInfinity);
        [Tooltip("Recharge speed ratio gained per every shield usage ratio consumed")]
        [SerializeField] private Stat rechargeRatioPerShield = new Stat(0.1f, 0f, float.Epsilon, float.PositiveInfinity);

        [field: Header("Consumption")]
        [Tooltip("Shield consumption ratio per tick")]
        [SerializeField] private Stat shieldUsageRatio = new Stat(0.1f, 0f, 0f, float.PositiveInfinity);

        [field: Header("Shield Consumption Ramp up")]
        [field: SerializeField] public int RampUpStartTick { get; private set; } = 8;
        [field: SerializeField] public float UsageRampUpRatio { get; private set; } = 0.01f;

        [field: Header("Shield Consumption Damage")]
        [field: SerializeField] public float Penetration { get; private set; } = 1f;
        [field: SerializeField] public float ProcCoefficient { get; private set; } = 0f;

        public override AbilityModule CreateModule()
        {
            return new Overclock(this, shieldUsageRatio.DeepCopy(), baseRechargeRatioBoost.DeepCopy(), rechargeRatioPerShield.DeepCopy());
        }
    }
}
