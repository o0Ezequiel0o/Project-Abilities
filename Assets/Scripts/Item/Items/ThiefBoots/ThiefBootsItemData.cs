using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Thief Boots", menuName = "ScriptableObjects/Items/Items/ThiefBoots", order = 1)]
    public class ThiefBootsItemData : ItemData
    {
        [field: Space]

        [field: SerializeField] public LayerMask HitLayers { get; private set; }
        [field: SerializeField] public float RequiredRadius { get; private set; }
        [field: SerializeField] public float RequiredTime { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat SpeedMultiplier { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new ThiefBootsItem(this, itemHandler, source);
        }
    }
}