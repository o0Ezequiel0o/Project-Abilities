using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Electric Heart", menuName = "ScriptableObjects/Items/Items/ElectricHeart", order = 1)]
    public class ElectricHeartItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat HealthRegen { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ShieldRegen { get; private set; }
        [field: SerializeField] public float ShieldRegenInterval { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new ElectricHeartItem(this, itemHandler, source);
        }

        protected override string GenerateDetailedStatDescription(int stacks)
        {
            return $"<style=\"Scaled Stat\">+{HealthRegen.GetValue(stacks):0.##}</style> health regen | " +
                $"<style=\"Scaled Stat\">+{ShieldRegen.GetValue(stacks):0.##}</style> shield regen.";
        }
    }
}