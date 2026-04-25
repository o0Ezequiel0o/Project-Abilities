using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Paycheck", menuName = "ScriptableObjects/Items/Items/Paycheck", order = 1)]
    public class PaycheckItemData : ItemData
    {
        [field: Space]

        [field: SerializeField] public float GoldRequiredForStack { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat DamageMultPerStack { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat DamageMultCap { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new PaycheckItem(this, itemHandler, source);
        }
    }
}