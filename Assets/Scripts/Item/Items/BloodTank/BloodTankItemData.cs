using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Blood Tank", menuName = "ScriptableObjects/Items/Items/BloodTank", order = 1)]
    public class BloodTankItemData : ItemData
    {
        [field: Space]

        [field: SerializeField] public float DamageReductionRatio { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat HealthInheritRatio { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new BloodTankItem(this, itemHandler, source);
        }
    }
}