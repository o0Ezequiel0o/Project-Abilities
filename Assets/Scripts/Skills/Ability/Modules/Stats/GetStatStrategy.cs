using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
    public abstract class GetStatStrategy
    {
        public abstract GetStatStrategy DeepCopy();

        public abstract Stat GetStat(GameObject source);
    }
}