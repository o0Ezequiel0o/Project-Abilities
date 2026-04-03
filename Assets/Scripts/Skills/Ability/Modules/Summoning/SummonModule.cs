using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Summoning
{
    [Serializable]
    public abstract class SummonModule
    {
        public abstract SummonModule DeepCopy();

        public abstract void OnSummonSpawn(GameObject summon, GameObject source);

        public abstract void OnDestroy(GameObject summon, GameObject source);

        public virtual void Upgrade() { }
    }
}