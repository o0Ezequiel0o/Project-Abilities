using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Hollow Point", menuName = "ScriptableObjects/Items/Items/HollowPoint", order = 1)]
    public class HollowPointItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat FlatMultDamage { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new HollowPointItem(this, itemHandler, source);
        }
    }
}