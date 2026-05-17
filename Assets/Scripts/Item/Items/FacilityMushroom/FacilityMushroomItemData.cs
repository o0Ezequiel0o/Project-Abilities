using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Facility Mushroom", menuName = "ScriptableObjects/Items/Items/FacilityMushroom", order = 1)]
    public class FacilityMushroomItemData : ItemData
    {
        [field: Space]

        [field: SerializeField] public DamageAreaEffect Prefab { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ProcChance { get; private set; }

        [field: Space]

        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat DamageMultiplier { get; private set; }
        [field: SerializeField] public float ArmorPenetration { get; private set; } = 0f;
        [field: SerializeField] public float ProcCoefficient { get; private set; } = 0f;

        [field: Space]

        [field: SerializeField] public float TickInterval { get; private set; } = 1f;
        [field: SerializeField] public int Ticks { get; private set; } = 3;

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new FacilityMushroomItem(this, itemHandler, source);
        }
    }
}