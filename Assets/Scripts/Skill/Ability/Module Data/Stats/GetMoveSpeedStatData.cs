using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
    public class GetMoveSpeedStatData : GetStatStrategyData
    {
        public override GetStatStrategy CreateStatStrategy()
        {
            return new GetMoveSpeedStat(this);
        }
    }
}