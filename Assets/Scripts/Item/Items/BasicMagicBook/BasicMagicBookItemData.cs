using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Basic Magic Book", menuName = "ScriptableObjects/Items/Items/BasicMagicBook", order = 1)]
    public class BasicMagicBookItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat Levels { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new BasicMagicBookItem(this, itemHandler, source);
        }
    }
}