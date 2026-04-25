using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "WD-04", menuName = "ScriptableObjects/Items/Items/WD04", order = 1)]
    public class WD04ItemData : ItemData
    {
        [field: Space]

        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ExtraMoveSpeed { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ExtraRotationSpeed { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new WD04Item(this, itemHandler, source);
        }
    }
}