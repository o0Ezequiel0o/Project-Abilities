using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Unstable Cell", menuName = "ScriptableObjects/Items/Items/UnstableCell", order = 1)]
    public class UnstableCellItemData : ItemData
    {
        [field: Space]

        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ExtraShield { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ExplosionCooldown { get; private set; }

        [field: Space]

        [field: SerializeField] public LayerMask HitLayers { get; private set; }
        [field: SerializeField] public LayerMask BlockLayers { get; private set; }
        [field: SerializeField] public float ExplosionRadius { get; private set; }
        [field: SerializeField] public float ExplosionKnockback { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new UnstableCellItem(this, itemHandler, source);
        }

        protected override string GenerateDetailedStatDescription(int stacks)
        {
            return $"<style=\"Scaled Stat\">+{ExtraShield.GetValue(stacks):0.##}</style> shield | " +
                $"<style=\"Scaled Stat\">{ExplosionCooldown.GetValue(stacks):0.##}s</style> explosion cooldown.";
        }
    }
}