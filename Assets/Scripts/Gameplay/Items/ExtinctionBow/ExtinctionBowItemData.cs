using System.Collections.Generic;
using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Extinction Bow", menuName = "ScriptableObjects/Items/Items/ExtinctionBow", order = 1)]
    public class ExtinctionBowItemData : ItemData
    {
        [field: Space]

        [field: SerializeField] public List<EntityType> Types = new List<EntityType>();
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat DamageMultiplier { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new ExtinctionBowItem(this, itemHandler, source);
        }

        protected override string GenerateDetailedStatDescription(int stacks)
        {
            return $"<style=\"Scaled Stat\">{DamageMultiplier.GetValue(stacks) * 100f:0.##}%</style> damage multiplier.";
        }
    }
}