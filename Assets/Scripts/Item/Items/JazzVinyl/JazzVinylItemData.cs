using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Jazz Vinyl", menuName = "ScriptableObjects/Items/Items/JazzVinyl", order = 1)]
    public class JazzVinylItemData : ItemData
    {
        [field: Space]

        [field: SerializeField] public StatusEffectData StatusEffect { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ProcChance { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new JazzVinylItem(this, itemHandler, source);
        }
    }
}