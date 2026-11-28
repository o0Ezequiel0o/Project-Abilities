using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Drill", menuName = "ScriptableObjects/Items/Items/Drill", order = 1)]
    public class DrillItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat FlatMultDamage { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat MaxStacks { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new DrillItem(this, itemHandler, source);
        }        
        
        protected override string GenerateDetailedStatDescription(int stacks)
        {
            if (stacks <= 0) return string.Empty;

            float damagePerHit = FlatMultDamage.GetValue(stacks);
            float maxStacks = MaxStacks.GetValue(stacks);

            float maxDamageBonus = damagePerHit * maxStacks;

            return $"<style=\"Scaled Stat\">+{damagePerHit * 100f:0.##}%</style> flat damage multiplier</style> / <style=\"Scaled Stat\">{maxDamageBonus * 100f:0.##}%</style> max.";
        }
    }
}