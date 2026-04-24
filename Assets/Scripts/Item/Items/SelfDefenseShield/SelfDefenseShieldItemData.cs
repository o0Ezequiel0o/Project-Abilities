using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Self-Defense Shield", menuName = "ScriptableObjects/Items/Items/SelfDefenseShield", order = 1)]
    public class SelfDefenseShieldItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ExtraShield { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new SelfDefenseShieldItem(this, itemHandler, source);
        }
    }
}