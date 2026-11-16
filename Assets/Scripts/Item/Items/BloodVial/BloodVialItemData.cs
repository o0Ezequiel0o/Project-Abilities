using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Blood Vial", menuName = "ScriptableObjects/Items/Items/BloodVial", order = 1)]
    public class BloodVialItemData : ItemData
    {
        [field: Space]

        [field: SerializeField] public float Cooldown { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat Healing { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat HealReceivedExtraMultPerStack { get; private set; }
        [field: SerializeField] public float ProcCoefficient { get; private set; } = 1f;
        [field: SerializeField] public float MissingHealthRatioForStack { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new BloodVialItem(this, itemHandler, source);
        }

        protected override string GenerateDetailedStatDescription(int stacks)
        {
            return $"<style=\"Scaled Stat\">{Healing.GetValue(stacks):0.##}</style> healing | " +
                $"<style=\"Scaled Stat\">+{HealReceivedExtraMultPerStack.GetValue(stacks) * 100f:0.##}%</style> healing.";
        }
    }
}