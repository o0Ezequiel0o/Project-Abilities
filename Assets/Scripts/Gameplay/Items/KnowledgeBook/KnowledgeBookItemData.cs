using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Book of Knowledge", menuName = "ScriptableObjects/Items/Items/KnowledgeBook", order = 1)]
    public class KnowledgeBookItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat XPMult { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ExtraMultPerTome { get; private set; }

        [field: Space]

        [field: SerializeField] public ItemData TomeItem { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new KnowledgeBookItem(this, itemHandler, source);
        }

        protected override string GenerateDetailedStatDescription(int stacks)
        {
            return $"<style=\"Scaled Stat\">{XPMult.GetValue(stacks) * 100f:0.##}%</style> experience multiplier " +
                $"<style=\"Scaled Stat\">+{ExtraMultPerTome.GetValue(stacks) * 100f:0.##}%</style> per tome.";
        }
    }
}