using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Leech", menuName = "ScriptableObjects/Items/Items/Leech", order = 1)]
    public class LeechItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat DamageHealRatio { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new LeechItem(this, itemHandler, source);
        }
    }
}