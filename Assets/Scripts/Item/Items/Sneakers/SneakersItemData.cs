using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Sneakers", menuName = "ScriptableObjects/Items/Items/Sneakers", order = 1)]
    public class SneakersItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ExtraMoveSpeed { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new SneakersItem(this, itemHandler, source);
        }
    }
}