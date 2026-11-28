using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
    public class GetHealthRegenStatData : GetStatStrategyData
    {
        public override GetStatStrategy CreateStatStrategy()
        {
            return new GetHealthRegenStat(this);
        }
    }
}