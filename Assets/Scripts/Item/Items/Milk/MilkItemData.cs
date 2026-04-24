using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Milk", menuName = "ScriptableObjects/Items/Items/Milk", order = 1)]
    public class MilkItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat DamageFlatMult;

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new MilkItem(this, itemHandler, source);
        }
    }
}