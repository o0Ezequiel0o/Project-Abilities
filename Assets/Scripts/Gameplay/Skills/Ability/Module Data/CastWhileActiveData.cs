using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class CastWhileActiveData : AbilityModuleData
    {
        [SerializeField] private Stat inactiveLength;
        [SerializeField] private Stat activeLength;
        [field: SerializeField] public CastWhileActive.InternalLoopState StartState { get; private set; }
        [SerializeReferenceDropdown, SerializeReference] private AbilityModuleData module;

        public override AbilityModule CreateModule()
        {
            return new CastWhileActive(this, inactiveLength.DeepCopy(), activeLength.DeepCopy(), module.CreateModule());
        }
    }
}