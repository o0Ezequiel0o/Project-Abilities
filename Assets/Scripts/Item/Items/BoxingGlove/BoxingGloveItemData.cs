using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Boxing Glove", menuName = "ScriptableObjects/Items/Items/BoxingGlove", order = 1)]
    public class BoxingGloveItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat Knockback { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new BoxingGloveItem(this, itemHandler, source);
        }
    }
}