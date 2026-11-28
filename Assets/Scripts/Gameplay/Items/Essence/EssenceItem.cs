using UnityEngine;

namespace Zeke.Items
{
    public class EssenceItem : Item
    {
        public override ItemData Data => data;
        private readonly EssenceItemData data;

        public EssenceItem(EssenceItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
        }
    }
}