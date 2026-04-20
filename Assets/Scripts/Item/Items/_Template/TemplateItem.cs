using UnityEngine;

namespace Zeke.Items
{
    public class TemplateItem : Item
    {
        //Template
        public override ItemData Data => data;
        private readonly TemplateItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        public TemplateItem(TemplateItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void OnAdded() { }

        public override void OnRemoved() { }

        public override void OnStackAdded() { }

        public override void OnStackRemoved() { }

        public override void OnUpdate() { }
    }
}