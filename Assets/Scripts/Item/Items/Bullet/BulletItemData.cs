using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Bullet", menuName = "ScriptableObjects/Items/Items/Bullet", order = 1)]
    public class BulletItemData : ItemData
    {
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat FlatDamage { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new BulletItem(this, itemHandler, source);
        }
    }
}