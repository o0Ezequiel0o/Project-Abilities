using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Emergency Generator", menuName = "ScriptableObjects/Items/Items/EmergencyGenerator", order = 1)]
    public class EmergencyGeneratorItemData : ItemData
    {
        [field: Space]

        [field: SerializeField] public float Interval { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat Amount { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new EmergencyGeneratorItem(this, itemHandler, source);
        }
    }
}