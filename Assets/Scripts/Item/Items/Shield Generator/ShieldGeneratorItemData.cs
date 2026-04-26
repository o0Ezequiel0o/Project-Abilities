using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Shield Generator", menuName = "ScriptableObjects/Items/Items/ShieldGenerator", order = 1)]
    public class ShieldGeneratorItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ExtraShieldRegen { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new ShieldGeneratorItem(this, itemHandler, source);
        }
    }
}