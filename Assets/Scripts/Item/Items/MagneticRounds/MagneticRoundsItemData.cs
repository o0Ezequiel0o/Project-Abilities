using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Magnetic Rounds", menuName = "ScriptableObjects/Items/Items/MagneticRounds", order = 1)]
    public class MagneticRoundsItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat DamageMultiplier { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new MagneticRoundsItem(this, itemHandler, source);
        }

        protected override string GenerateDetailedStatDescription(int stacks)
        {
            return $"<style=\"Scaled Stat\">{DamageMultiplier.GetValue(stacks) * 100f:0.##}%</style> damage multiplier.";
        }
    }
}