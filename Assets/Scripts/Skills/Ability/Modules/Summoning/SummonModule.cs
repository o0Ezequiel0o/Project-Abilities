using UnityEngine;

namespace Zeke.Abilities.Modules.Summoning
{
    public abstract class SummonModule
    {
        public abstract void OnSummonSpawn(GameObject summon, GameObject source);

        public abstract void OnDestroy(GameObject summon, GameObject source);

        public virtual void Upgrade() { }
    }
}