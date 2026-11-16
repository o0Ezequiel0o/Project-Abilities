using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Monster Fangs", menuName = "ScriptableObjects/Items/Items/MonsterFangs", order = 1)]
    public class MonsterFangsItemData : ItemData
    {
        [field: Space]

        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ExtraHealth { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat MaxStacks { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new MonsterFangsItem(this, itemHandler, source);
        }

        protected override string GenerateDetailedStatDescription(int stacks)
        {
            return $"<style=\"Scaled Stat\">+{ExtraHealth.GetValue(stacks):0.##}</style> max health per stack | " +
                $"<style=\"Scaled Stat\">{Mathf.FloorToInt(MaxStacks.GetValue(stacks)):0.##}</style> max stacks.";
        }
    }
}