using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class PullToCenterData : AbilityModuleData
    {
        [SerializeField] private Stat Radius;

        [field: SerializeField] public float Force { get; private set; }
        [field: SerializeField] public LayerMask HitLayers { get; private set; }

        [field: Space]

        [field: SerializeField] public bool ScaleWithDistance { get; private set; } = true;

        public override AbilityModule CreateModule()
        {
            return new PullToCenter(this, Radius.DeepCopy());
        }
    }
}