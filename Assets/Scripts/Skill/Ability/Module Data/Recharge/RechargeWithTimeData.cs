using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class RechargeWithTimeData : RechargeTypeData
    {
        public override RechargeType CreateRechargeTypeModule()
        {
            return new RechargeWithTime();
        }
    }
}