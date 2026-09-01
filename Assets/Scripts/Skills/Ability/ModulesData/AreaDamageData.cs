using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class AreaDamageData : AbilityModuleData
    {
        [SerializeField] private Stat radius;
        [SerializeField] private Stat damage;

        [field: SerializeField] public float Knockback { get; private set; }
        [field: SerializeField] public LayerMask HitLayers { get; private set; }

        public override AbilityModule CreateModule()
        {
            return new AreaDamage(this, radius.DeepCopy(), damage.DeepCopy());
        }
    }
}