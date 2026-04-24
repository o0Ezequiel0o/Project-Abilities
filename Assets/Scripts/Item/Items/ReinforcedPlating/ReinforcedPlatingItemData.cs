using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Reinforced Plating", menuName = "ScriptableObjects/Items/Items/ReinforcedPlating", order = 1)]
    public class ReinforcedPlatingItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ExtraArmor { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new ReinforcedPlatingItem(this, itemHandler, source);
        }
    }
}