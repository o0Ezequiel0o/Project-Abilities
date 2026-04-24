using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Layered Armor", menuName = "ScriptableObjects/Items/Items/LayeredArmor", order = 1)]
    public class LayeredArmorItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat FlatDamageReduction { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new LayeredArmorItem(this, itemHandler, source);
        }
    }
}