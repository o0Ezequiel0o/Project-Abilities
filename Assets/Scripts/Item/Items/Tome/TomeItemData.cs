using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Tome", menuName = "ScriptableObjects/Items/Items/Tome", order = 1)]
    public class TomeItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat XPFlatMult { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new TomeItem(this, itemHandler, source);
        }

        protected override string GenerateDetailedStatDescription(int stacks)
        {
            return $"<style=\"Scaled Stat\">{XPFlatMult.GetValue(stacks) * 100f:0.##}%</style> flat experience multiplier.";
        }
    }
}