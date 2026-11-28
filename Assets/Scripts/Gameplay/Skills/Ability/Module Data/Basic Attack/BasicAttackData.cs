using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class BasicAttackData : AbilityModuleData
    {
        [SerializeField] private Stat damage;
        [field: SerializeField] public float ArmorPenetration { get; private set; }
        [field: SerializeField] public float ProcCoefficient { get; private set; }
        [field: SerializeField] public float Knockback { get; private set; }

        [Space]

        [SerializeReferenceDropdown, SerializeReference] private OverlapShape shape;
        [field: SerializeField] public LayerMask HitLayers { get; private set; }
        [field: SerializeField] public float CastOffset { get; private set; }
        [field: SerializeField] public bool CastAtSourceCenter { get; private set; }

        public override AbilityModule CreateModule()
        {
            return new BasicAttack(this, damage.DeepCopy(), shape.DeepCopy());
        }
    }
}