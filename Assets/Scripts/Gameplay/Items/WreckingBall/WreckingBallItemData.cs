using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Wrecking Ball", menuName = "ScriptableObjects/Items/Items/WreckingBall", order = 1)]
    public class WreckingBallItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ArmorPerMeter { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new WreckingBallItem(this, itemHandler, source);
        }

        protected override string GenerateDetailedStatDescription(int stacks)
        {
            return $"<style=\"Scaled Stat\">{ArmorPerMeter.GetValue(stacks):0.##}</style> armor per meter.";
        }
    }
}