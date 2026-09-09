using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Essence", menuName = "ScriptableObjects/Items/Items/Essence", order = 1)]
    public class EssenceItemData : ItemData
    {
        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new EssenceItem(this, itemHandler, source);
        }
    }
}