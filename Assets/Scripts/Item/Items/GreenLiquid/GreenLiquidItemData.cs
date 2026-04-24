using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Green Liquid", menuName = "ScriptableObjects/Items/Items/GreenLiquid", order = 1)]
    public class GreenLiquidItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ExtraHealthRegen { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new GreenLiquidItem(this, itemHandler, source);
        }
    }
}