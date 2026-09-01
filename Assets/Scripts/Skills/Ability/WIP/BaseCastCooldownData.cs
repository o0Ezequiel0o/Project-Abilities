using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class BaseCastCooldownData : AbilityModuleData
    {
        [SerializeField] private Stat cooldown = new Stat(0.05f, 0f, 0f, float.PositiveInfinity);

        public override AbilityModule CreateModule()
        {
            return new BaseCastCooldown(this, cooldown.DeepCopy());
        }
    }
}