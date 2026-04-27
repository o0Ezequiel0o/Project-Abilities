using UnityEngine;
using Zeke.Abilities;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Coffee", menuName = "ScriptableObjects/Items/Items/Coffee", order = 1)]
    public class CoffeeItemData : ItemData
    {
        [field: Space]

        [field: SerializeField] public AbilityType AbilityType { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ExtraChargeSpeed { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ExtraMoveSpeed { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new CoffeeItem(this, itemHandler, source);
        }
    }
}