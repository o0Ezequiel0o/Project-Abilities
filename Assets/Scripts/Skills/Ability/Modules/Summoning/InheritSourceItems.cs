using UnityEngine;
using System;
using Zeke.Items;

namespace Zeke.Abilities.Modules.Summoning
{
    [Serializable]
    public class InheritSourceItems : SummonModule
    {
        private readonly InheritSourceItemsData data;

        public InheritSourceItems(InheritSourceItemsData data)
        {
            this.data = data;
        }

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