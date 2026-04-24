using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Thorns", menuName = "ScriptableObjects/Items/Items/Thorns", order = 1)]
    public class ThornsItemData : ItemData
    {
        [field: Space]

        [field: SerializeField] public StatusEffectData Effect { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat Stacks { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new ThornsItem(this, itemHandler, source);
        }
    }
}