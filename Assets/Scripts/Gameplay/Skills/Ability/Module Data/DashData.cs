using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class DashData : AbilityModuleData
    {
        [field: SerializeField] public float JumpForce { get; private set; }
        [field: SerializeField] public Vector2 Direction { get; private set; }

        public override AbilityModule CreateModule()
        {
            return new Dash(this);
        }
    }
}