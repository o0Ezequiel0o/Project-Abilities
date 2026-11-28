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

        public override void Initialize() { }

        public override void OnRemoved() { }

        public override void OnStacksAdded(int amount) { }

        public override void OnStacksRemoved(int amount) { }

        public override void OnUpdate() { }
    }
}