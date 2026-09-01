using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class TakeDamageData : AbilityModuleData
    {
        [SerializeField] private Stat damage;
        [field: SerializeField] public ValueType ValueType { get; private set; }
        [field: SerializeField] public float ArmorPenetration { get; private set; }

        [field: SerializeField] public bool Lethal { get; private set; } = true;
        [field: SerializeField] public bool IgnoresShield { get; private set; } = false;

        public override AbilityModule CreateModule()
        {
            return new TakeDamage(this, damage.DeepCopy());
        }
    }
}