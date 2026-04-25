using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Warp Clock", menuName = "ScriptableObjects/Items/Items/WarpClock", order = 1)]
    public class WarpClockItemData : ItemData
    {
        [field: Space]

        [field: SerializeField, Min(0f)] public float StoreTime { get; private set; }
        [field: SerializeField, Min(0f)] public float TriggerTimeRequired { get; private set; }

        [field: Space]

        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat DamageHealRatio { get; private set; }
        [field: SerializeField] public float HealTickTime { get; private set; }
        [field: SerializeField, Min(1)] public int HealTicks { get; private set; }


        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new WarpClockItem(this, itemHandler, source);
        }
    }
}