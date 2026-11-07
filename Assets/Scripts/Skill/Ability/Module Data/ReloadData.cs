using UnityEngine;
using System;

using static Zeke.Abilities.Modules.Reload;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class ReloadData : AbilityModuleData
    {
        [SerializeReferenceDropdown, SerializeReference] private ReloadStrategyData strategy;

        [Serializable]
        public abstract class ReloadStrategyData
        {
            [field: SerializeField] public AbilityType AbilityType { get; private set; }
            [SerializeField] protected Stat chargesAmount;

            public abstract ReloadStrategy CreateStrategy();
        }

        public override AbilityModule CreateModule()
        {
            return new Reload(this, strategy.CreateStrategy());
        }

        [Serializable]
        public class OnCastData : ReloadStrategyData
        {
            public override ReloadStrategy CreateStrategy()
            {
                return new OnCast(this, chargesAmount.DeepCopy());
            }
        }

        [Serializable]
        public class OnDurationEndData : ReloadStrategyData
        {
            public override ReloadStrategy CreateStrategy()
            {
                return new OnDurationEnd(this, chargesAmount.DeepCopy());
            }
        }

        [Serializable]
        public class WhileActiveData : ReloadStrategyData
        {
            public override ReloadStrategy CreateStrategy()
            {
                return new WhileActive(this, chargesAmount.DeepCopy());
            }
        }
    }
}