using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Green Gem", menuName = "ScriptableObjects/Items/Items/GreenGem", order = 1)]
    public class GreenGemItemData : ItemData
    {
        [field: Space]

        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ExtraHealth { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ExtraHealthRegen { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new GreenGemItem(this, itemHandler, source);
        }
    }
}