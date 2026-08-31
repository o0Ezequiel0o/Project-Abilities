using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
    public class GetDamageReceivedMultiplierStatData : GetStatStrategyData
    {
        public override GetStatStrategy CreateStatStrategy()
        {
            return new GetDamageReceivedMultiplierStat(this);
        }
    }
}