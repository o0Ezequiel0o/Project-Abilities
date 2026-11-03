using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class CastTowardClosestData : AbilityModuleData
    {
        [SerializeReferenceDropdown, SerializeReference] private AbilityModuleData module;
        [field: SerializeField] public bool AlwaysCast { get; private set; }
        [field: SerializeField] public Vector2 Offset { get; private set; }

        [field: Space]

        [field: SerializeField] public LayerMask TargetLayer { get; private set; }
        [SerializeField] private Stat targetRadius;

        public override AbilityModule CreateModule()
        {
            return new CastTowardClosest(this, module.CreateModule(), targetRadius.DeepCopy());
        }
    }
}