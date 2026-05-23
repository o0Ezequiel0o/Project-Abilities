using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Blood Debt", menuName = "ScriptableObjects/Items/Items/BloodDebt", order = 1)]
    public class BloodDebtItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat Healing { get; private set; }
        [field: SerializeField] public float ProcCoefficient { get; private set; } = 1f;

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new BloodDebtItem(this, itemHandler, source);
        }
    }
}