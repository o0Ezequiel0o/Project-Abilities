using UnityEngine;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Cursed Skull", menuName = "ScriptableObjects/Items/Items/CursedSkull", order = 1)]
    public class CursedSkullItemData : ItemData
    {
        [field: Space]

        [field: SerializeField] public float Chance { get; private set; }
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat ExtraLevels { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new CursedSkullItem(this, itemHandler, source);
        }
    }
}