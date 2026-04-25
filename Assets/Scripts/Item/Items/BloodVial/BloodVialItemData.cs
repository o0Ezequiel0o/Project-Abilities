using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Blood Vial", menuName = "ScriptableObjects/Items/Items/BloodVial", order = 1)]
    public class BloodVialItemData : ItemData
    {
        [field: Space]

        [field: SerializeField] public float Cooldown { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat Healing { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new BloodVialItem(this, itemHandler, source);
        }
    }
}