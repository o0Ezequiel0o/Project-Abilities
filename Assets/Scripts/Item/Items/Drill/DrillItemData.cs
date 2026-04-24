using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Drill", menuName = "ScriptableObjects/Items/Items/Drill", order = 1)]
    public class DrillItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat FlatMultDamage { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new DrillItem(this, itemHandler, source);
        }
    }
}