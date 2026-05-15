using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Magnetic Plating", menuName = "ScriptableObjects/Items/Items/Magnetic Plating", order = 1)]
    public class MagneticPlatingItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ExtraShield { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ExtraArmor { get; private set; }

        [field: Space]

        [field: SerializeField] public float ShieldRatioRequired { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new MagneticPlatingItem(this, itemHandler, source);
        }
    }
}