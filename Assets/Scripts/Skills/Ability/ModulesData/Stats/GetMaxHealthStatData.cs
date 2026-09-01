using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
    public class GetMaxHealthStatData : GetStatStrategyData
    {
        public override GetStatStrategy CreateStatStrategy()
        {
            return new GetMaxHealthStat(this);
        }
    }
}