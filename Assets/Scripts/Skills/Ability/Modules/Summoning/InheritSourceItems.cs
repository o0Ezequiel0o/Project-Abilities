using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Summoning
{
    [Serializable]
    public class InheritSourceItems : SummonModule
    {
        public override SummonModule DeepCopy() => new InheritSourceItems();

        public override void OnSummonSpawn(GameObject summon, GameObject source)
        {
            if (summon.TryGetComponent(out ItemHandler summonItemHandler) && source.TryGetComponent(out ItemHandler sourceItemHandler))
            {
                summonItemHandler.AddItems(sourceItemHandler.Items);
            }
        }

        public override void OnDestroy(GameObject summon, GameObject source) { }
    }
}