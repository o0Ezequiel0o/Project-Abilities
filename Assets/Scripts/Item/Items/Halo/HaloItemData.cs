using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Halo", menuName = "ScriptableObjects/Items/Items/Halo", order = 1)]
    public class HaloItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat HealthReceivedMult { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new HaloItem(this, itemHandler, source);
        }

        protected override string GenerateDetailedStatDescription(int stacks)
        {
            return $"<style=\"Scaled Stat\">{HealthReceivedMult.GetValue(stacks) * 100f:0.##}%</style> healing received multiplier.";
        }
    }
}