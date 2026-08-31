using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
    public class GetRotationSpeedStatData : GetStatStrategyData
    {
        public override GetStatStrategy CreateStatStrategy()
        {
            return new GetRotationSpeedStat(this);
        }
    }
}