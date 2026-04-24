using UnityEngine;
using Zeke.Abilities;

namespace Zeke.Items
{
    [CreateAssetMenu(fileName = "Bomb Bag", menuName = "ScriptableObjects/Items/Items/BombBag", order = 1)]
    public class BombBagItemData : ItemData
    {
        [field: Space]
        [field: SerializeField] public BombItemBomb BombPrefab { get; private set; }
        [field: SerializeField] public AbilityType AbilityType { get; private set; }

        [field: Space]

        [field: SerializeReferenceDropdown, SerializeReference] public IStackStat Damage { get; private set; }
        [field: SerializeField] public float Cooldown { get; private set; }
        [field: SerializeField] public float Radius { get; private set; }
        [field: SerializeField] public float Fuse { get; private set; }

        public override Item CreateItem(ItemHandler itemHandler, GameObject source)
        {
            return new BombBagItem(this, itemHandler, source);
        }
    }
}