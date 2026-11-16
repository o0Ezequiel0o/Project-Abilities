using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Leech", menuName = "ScriptableObjects/Items/Items/Leech", order = 1)]
    public class LeechItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat DamageHealRatio { get; private set; }
        [field: SerializeField] public float ProcCoefficient { get; private set; } = 1f;

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new LeechItem(this, itemHandler, source);
        }

        protected override string GenerateDetailedStatDescription(int stacks)
        {
            return $"<style=\"Scaled Stat\">{DamageHealRatio.GetValue(stacks) * 100:0.##}%</style> healing.";
        }
    }
}