using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Riot Shield", menuName = "ScriptableObjects/Items/Items/RiotShield", order = 1)]
    public class RiotShieldItemData : ItemData
    {
        [field: Space]

        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ExtraShield { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ExtraShieldRegen { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new RiotShieldItem(this, itemHandler, source);
        }
    }
}