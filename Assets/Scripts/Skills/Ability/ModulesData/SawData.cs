using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class SawData : AbilityModuleData
    {
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField] public float CastDistance { get; private set; }
        [field: SerializeField] public float DamageRadius { get; private set; }

        [field: Space]

        [field: SerializeField] public float ProcCoefficient { get; private set; }
        [field: SerializeField] public float ArmorPenetration { get; private set; }

        [field: Space]

        [field: SerializeField] public LayerMask HitLayers { get; private set; }
        [field: SerializeField] public LayerMask BlockLayers { get; private set; }

        [Space]

        [SerializeField] private Stat damage;
        [SerializeField] private Stat damageCooldown;

        [field: Space]

        [field: SerializeField] public StatusEffectData Effect { get; private set; }
        [field: SerializeField] public int EffectProcChance { get; private set; }

        public override AbilityModule CreateModule()
        {
            return new Saw(this, damage.DeepCopy(), damageCooldown.DeepCopy());
        }
    }
}