using UnityEngine;
using Zeke.Abilities;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Military Manual", menuName = "ScriptableObjects/Items/Items/MilitaryManual", order = 1)]
    public class MilitaryManualItemData : ItemData
    {
        [field: Space]

        [field: SerializeField] public AbilityType AbilityType { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ExtraChargeSpeed { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat FlatMultDamage { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new MilitaryManualItem(this, itemHandler, source);
        }
    }
}