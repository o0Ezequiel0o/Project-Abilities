using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
    public class GetArmorStatData : GetStatStrategyData
    {
        public override GetStatStrategy CreateStatStrategy()
        {
            return new GetArmorStat(this);
        }
    }
}