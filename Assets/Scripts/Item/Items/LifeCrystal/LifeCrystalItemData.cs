using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Life Crystal", menuName = "ScriptableObjects/Items/Items/LifeCrystal", order = 1)]
    public class LifeCrystalItemData : ItemData
    {
        [field: Space]

        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ExtraHealth { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ExtraHealthRegen { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new LifeCrystalItem(this, itemHandler, source);
        }
    }
}