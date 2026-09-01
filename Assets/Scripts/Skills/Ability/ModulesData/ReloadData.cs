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
            [SerializeReferenceDropdown, SerializeReference] protected GetAbilityStrategyData strategy = new GetAbilityTypeData();
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
                return new OnCast(this, strategy.CreateStrategy(), chargesAmount.DeepCopy());
            }
        }

        [Serializable]
        public class OnDurationEndData : ReloadStrategyData
        {
            public override ReloadStrategy CreateStrategy()
            {
                return new OnDurationEnd(this, strategy.CreateStrategy(), chargesAmount.DeepCopy());
            }
        }

        [Serializable]
        public class WhileActiveData : ReloadStrategyData
        {
            public override ReloadStrategy CreateStrategy()
            {
                return new WhileActive(this, strategy.CreateStrategy(), chargesAmount.DeepCopy());
            }
        }
    }
}