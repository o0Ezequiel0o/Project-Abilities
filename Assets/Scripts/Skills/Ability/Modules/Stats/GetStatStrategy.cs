using UnityEngine;

namespace Zeke.Abilities.Modules.Stats
{
    public abstract class GetStatStrategy
    {
        public abstract GetStatStrategy DeepCopy();

        public abstract Stat GetStat(GameObject source);
    }
}