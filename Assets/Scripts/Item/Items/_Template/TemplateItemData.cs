using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Template", menuName = "ScriptableObjects/Items/Items/Template", order = 1)]
    public class TemplateItemData : ItemData
    {
        //Template
        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new TemplateItem(this, itemHandler, source);
        }
    }
}