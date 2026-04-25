using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Protein Bar", menuName = "ScriptableObjects/Items/Items/ProteinBar", order = 1)]
    public class ProteinBarItemData : ItemData
    {
        [field: Space]

        [field: SerializeField] public float DistanceRequired { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat Healing { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new ProteinBarItem(this, itemHandler, source);
        }
    }
}