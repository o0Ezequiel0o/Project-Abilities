using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
    public class GetHealingReceivedMultiplierStatData : GetStatStrategyData
    {
        public override GetStatStrategy CreateStatStrategy()
        {
            return new GetHealingReceivedMultiplierStat(this);
        }
    }
}