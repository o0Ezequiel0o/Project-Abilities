using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class RecoilData : AbilityModuleData
    {
        [SerializeField] private Stat force;
        [field: SerializeField] public Vector2 Direction { get; private set; }

        public override AbilityModule CreateModule()
        {
            return new Recoil(this, force.DeepCopy());
        }
    }
}