using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
    public abstract class GetStatStrategyData
    {
        public abstract GetStatStrategy CreateStatStrategy();
    }
}