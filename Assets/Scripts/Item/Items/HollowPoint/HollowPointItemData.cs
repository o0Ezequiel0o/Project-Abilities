using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Hollow Point", menuName = "ScriptableObjects/Items/Items/HollowPoint", order = 1)]
    public class HollowPointItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat FlatMultDamage { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat FlatDamage { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new HollowPointItem(this, itemHandler, source);
        }

        protected override string GenerateDetailedStatDescription(int stacks)
        {
            return $"<style=\"Scaled Stat\">{FlatMultDamage.GetValue(stacks) * 100f:0.##}%</style> flat damage multiplier | " +
                $"<style=\"Scaled Stat\">{FlatDamage.GetValue(stacks):0.##}</style> flat damage.";
        }
    }
}