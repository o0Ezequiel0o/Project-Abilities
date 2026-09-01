using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public abstract class RechargeTypeData
    {
        public abstract RechargeType CreateRechargeTypeModule();
    }
}