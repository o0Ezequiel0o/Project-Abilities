using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Black Blood", menuName = "ScriptableObjects/Items/Items/BlackBlood", order = 1)]
    public class BlackBloodItemData : ItemData
    {
        [field: Space]

        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat Cooldown { get; private set; }
        [field: SerializeField] public float DamageReductionRatio { get; private set; }

        [field: Space] 
        [field: SerializeField] public StatusEffectData DisplayEffect { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new BlackBloodItem(this, itemHandler, source);
        }
    }
}