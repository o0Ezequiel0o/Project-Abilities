using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Dice", menuName = "ScriptableObjects/Items/Items/Dice", order = 1)]
    public class DiceItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ExtraLuck { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new DiceItem(this, itemHandler, source);
        }
    }
}