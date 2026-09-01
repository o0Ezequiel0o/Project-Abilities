using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class CastOffsetData : AbilityModuleData
    {
        [field: SerializeField] public Vector2 Offset { get; private set; }
        [field: SerializeField] public float Angle { get; private set; }
        [SerializeReference, SerializeReferenceDropdown] private AbilityModuleData module;

        public override AbilityModule CreateModule()
        {
            return new CastOffset(this, module.CreateModule());
        }
    }
}