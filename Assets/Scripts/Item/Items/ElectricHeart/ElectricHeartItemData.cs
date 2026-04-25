using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Electric Heart", menuName = "ScriptableObjects/Items/Items/ElectricHeart", order = 1)]
    public class ElectricHeartItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat HealthRegen { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new ElectricHeartItem(this, itemHandler, source);
        }
    }
}